using ProductManagement_API.Models.DTOs;
using ProductManagement_API.Models.Entities;
using ProductManagement_API.Repositories.IRepository;
using ProductManagement_API.Services.IService;

namespace ProductManagement_API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return MapToDto(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(MapToDto);
        }

        public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
        {
            var categories = await _categoryRepository.GetActiveAsync();
            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
        {
            var nameExists = await _categoryRepository.ExistsByNameAsync(createDto.CategoryName);
            if (nameExists)
            {
                throw new InvalidOperationException($"Category with name '{createDto.CategoryName}' already exists");
            }

            var category = new Category
            {
                CategoryName = createDto.CategoryName,
                Description = createDto.Description,
                IsActive = true
            };

            var createdCategory = await _categoryRepository.CreateAsync(category);
            return MapToDto(createdCategory);
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null) return null;

            if (updateDto.CategoryName != null && updateDto.CategoryName != existingCategory.CategoryName)
            {
                var nameExists = await _categoryRepository.ExistsByNameAsync(updateDto.CategoryName);
                if (nameExists)
                {
                    throw new InvalidOperationException($"Category with name '{updateDto.CategoryName}' already exists");
                }
            }

            if (updateDto.CategoryName != null)
                existingCategory.CategoryName = updateDto.CategoryName;
            if (updateDto.Description != null)
                existingCategory.Description = updateDto.Description;
            if (updateDto.IsActive.HasValue)
                existingCategory.IsActive = updateDto.IsActive.Value;

            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);
            return MapToDto(updatedCategory);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _categoryRepository.DeleteAsync(id);
        }

        public async Task<int> GetProductCountAsync(int categoryId)
        {
            return await _categoryRepository.GetProductCountAsync(categoryId);
        }

        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                IsActive = category.IsActive,
                ProductCount = category.Products?.Count(p => p.IsActive) ?? 0,
                CreatedAt = category.CreatedAt
            };
        }
    }
}