// Navbar scroll effect
window.addEventListener('scroll', function () {
    const navbar = document.querySelector('.custom-navbar');
    if (window.scrollY > 50) {
        navbar.style.background = 'rgba(255, 255, 255, 0.98)';
        navbar.style.boxShadow = '0 2px 20px rgba(0, 0, 0, 0.1)';
    } else {
        navbar.style.background = 'rgba(255, 255, 255, 0.95)';
        navbar.style.boxShadow = '0 10px 30px rgba(0, 0, 0, 0.1)';
    }
});

// Smooth scrolling for navigation links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// Counter animation
function animateCounters() {
    const counters = document.querySelectorAll('.stat-item h3');
    counters.forEach(counter => {
        const target = parseInt(counter.textContent.replace(/[^0-9]/g, ''));
        const increment = target / 100;
        let count = 0;

        const updateCounter = () => {
            if (count < target) {
                count += increment;
                if (counter.textContent.includes('+')) {
                    counter.textContent = Math.ceil(count) + '+';
                } else if (counter.textContent.includes('%')) {
                    counter.textContent = Math.ceil(count) + '%';
                } else {
                    counter.textContent = Math.ceil(count) + ' Năm';
                }
                setTimeout(updateCounter, 20);
            }
        };
        updateCounter();
    });
}

// Trigger counter animation when in view
const observerOptions = {
    threshold: 0.5,
    triggerOnce: true
};

const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            animateCounters();
        }
    });
}, observerOptions);

const heroStats = document.querySelector('.hero-stats');
if (heroStats) {
    observer.observe(heroStats);
}

// Product card hover effects
document.querySelectorAll('.product-card').forEach(card => {
    card.addEventListener('mouseenter', function () {
        this.style.transform = 'translateY(-10px) scale(1.02)';
    });

    card.addEventListener('mouseleave', function () {
        this.style.transform = 'translateY(0) scale(1)';
    });
});

// Newsletter subscription
document.querySelector('.input-group button')?.addEventListener('click', function () {
    const email = this.previousElementSibling.value;
    if (email && email.includes('@')) {
        alert('Cảm ơn bạn đã đăng ký nhận tin!');
        this.previousElementSibling.value = '';
    } else {
        alert('Vui lòng nhập email hợp lệ!');
    }
});

// Loading animation
window.addEventListener('load', function () {
    document.body.style.opacity = '0';
    document.body.style.transition = 'opacity 0.5s ease-in-out';
    setTimeout(() => {
        document.body.style.opacity = '1';
    }, 100);
});