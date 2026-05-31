// Mini-Mart POS System - Main JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
    
    // Update time display
    updateTime();
    setInterval(updateTime, 1000);
    
    // Add click handlers for action buttons
    document.querySelectorAll('.action-btn').forEach(btn => {
        btn.addEventListener('click', function(e) {
            e.preventDefault();
            const action = this.querySelector('span').textContent;
            handleQuickAction(action);
        });
    });
    
    // Add hover effects to stat cards
    document.querySelectorAll('.stat-card').forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-5px)';
        });
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0)';
        });
    });
});

function updateTime() {
    const now = new Date();
    const timeString = now.toLocaleTimeString('en-US', { 
        hour: '2-digit', 
        minute: '2-digit',
        second: '2-digit'
    });
    // Update any time displays if needed
}

function handleQuickAction(action) {
    switch(action) {
        case 'Add Product':
            showNotification('Opening Product Management...', 'info');
            break;
        case 'New Sale':
            showNotification('Opening POS Billing...', 'info');
            break;
        case 'Stock In':
            showNotification('Opening Stock Management...', 'info');
            break;
        case 'Reports':
            showNotification('Opening Reports...', 'info');
            break;
        case 'Backup':
            showNotification('Starting backup process...', 'warning');
            break;
        case 'QR Payment':
            showNotification('Opening QR Payment...', 'info');
            break;
        default:
            showNotification('Action not implemented yet', 'warning');
    }
}

function showNotification(message, type = 'info') {
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} alert-dismissible fade show notification-toast`;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
        min-width: 300px;
        animation: slideIn 0.3s ease-out;
    `;
    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    document.body.appendChild(notification);
    
    // Auto dismiss after 3 seconds
    setTimeout(() => {
        notification.classList.remove('show');
        setTimeout(() => {
            notification.remove();
        }, 300);
    }, 3000);
}

// Add slide-in animation
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
`;
document.head.appendChild(style);

// Simulate real-time updates (for demo purposes)
function simulateRealTimeUpdates() {
    // Update sales numbers randomly
    setInterval(() => {
        const salesElement = document.querySelector('.sales-card .card-text');
        if (salesElement) {
            const currentSales = parseInt(salesElement.textContent.replace(/[^0-9]/g, ''));
            const newSales = currentSales + Math.floor(Math.random() * 100);
            salesElement.textContent = `Rs. ${newSales.toLocaleString()}`;
        }
    }, 30000);
}

simulateRealTimeUpdates();
