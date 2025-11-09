// SpaceEventHub - Main Application JavaScript

// Configuration
// Quando rodando no Docker, o nginx faz proxy de /api/ para o backend
// Então usamos URL relativa para funcionar em qualquer ambiente
const API_BASE_URL = window.location.origin + '/api';

// Authentication
function getToken() {
    return localStorage.getItem('token');
}

function setToken(token) {
    localStorage.setItem('token', token);
}

function removeToken() {
    localStorage.removeItem('token');
}

function getCurrentUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
}

function setCurrentUser(user) {
    localStorage.setItem('user', JSON.stringify(user));
}

function removeCurrentUser() {
    localStorage.removeItem('user');
}

function isAuthenticated() {
    return !!getToken();
}

function logout() {
    removeToken();
    removeCurrentUser();
    window.location.href = 'index.html';
}

// API Request Helper
async function apiRequest(endpoint, options = {}) {
    const url = `${API_BASE_URL}${endpoint}`;
    const token = getToken();

    const headers = {
        'Content-Type': 'application/json',
        ...options.headers
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const config = {
        ...options,
        headers
    };

    try {
        const response = await fetch(url, config);

        if (response.status === 401) {
            // Unauthorized - clear auth and redirect to login
            removeToken();
            removeCurrentUser();
            if (window.location.pathname !== '/login.html' && window.location.pathname !== '/register.html' && window.location.pathname !== '/index.html') {
                window.location.href = 'login.html';
            }
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
        }

        // Handle 204 No Content
        if (response.status === 204) {
            return null;
        }

        return await response.json();
    } catch (error) {
        console.error('API request failed:', error);
        throw error;
    }
}

// Initialize Navigation
function initializeNav() {
    const loginLink = document.getElementById('loginLink');
    const registerLink = document.getElementById('registerLink');
    const logoutBtn = document.getElementById('logoutBtn');
    const dashboardLink = document.getElementById('dashboardLink');
    const createEventLink = document.getElementById('createEventLink');
    const notificationIcon = document.getElementById('notificationIcon');

    if (isAuthenticated()) {
        if (loginLink) loginLink.style.display = 'none';
        if (registerLink) registerLink.style.display = 'none';
        if (logoutBtn) {
            logoutBtn.style.display = 'block';
            logoutBtn.addEventListener('click', logout);
        }
        if (dashboardLink) dashboardLink.style.display = 'block';
        
        // Show create event link for Organizers and Admins
        if (createEventLink) {
            const user = getCurrentUser();
            if (user && (user.role === 'Organizer' || user.role === 'Admin')) {
                createEventLink.style.display = 'block';
            } else {
                createEventLink.style.display = 'none';
            }
        }
        
        if (notificationIcon) {
            notificationIcon.style.display = 'block';
            initializeNotifications();
        }
    } else {
        if (loginLink) loginLink.style.display = 'block';
        if (registerLink) registerLink.style.display = 'block';
        if (logoutBtn) logoutBtn.style.display = 'none';
        if (dashboardLink) dashboardLink.style.display = 'none';
        if (createEventLink) createEventLink.style.display = 'none';
        if (notificationIcon) notificationIcon.style.display = 'none';
    }
}

// Notifications
let notificationInterval;

function initializeNotifications() {
    const notificationIcon = document.getElementById('notificationIcon');
    const notificationDropdown = document.getElementById('notificationDropdown');

    if (notificationIcon && notificationDropdown) {
        // Click on icon to toggle dropdown
        notificationIcon.addEventListener('click', (e) => {
            e.stopPropagation();
            const isOpen = notificationDropdown.classList.contains('show');
            
            if (isOpen) {
                notificationDropdown.classList.remove('show');
            } else {
                notificationDropdown.classList.add('show');
                loadNotifications();
            }
        });

        // Close dropdown when clicking outside
        document.addEventListener('click', (e) => {
            // Check if click is outside both icon and dropdown
            const clickedIcon = notificationIcon.contains(e.target);
            const clickedDropdown = notificationDropdown.contains(e.target);
            
            if (!clickedIcon && !clickedDropdown) {
                notificationDropdown.classList.remove('show');
            }
        });

        // Prevent dropdown from closing when clicking inside it
        notificationDropdown.addEventListener('click', (e) => {
            e.stopPropagation();
        });

        const markAllReadBtn = document.getElementById('markAllReadBtn');
        if (markAllReadBtn) {
            markAllReadBtn.addEventListener('click', (e) => {
                e.stopPropagation();
                markAllNotificationsAsRead();
            });
        }
    }

    // Load unread count
    loadUnreadCount();

    // Poll for new notifications every 30 seconds
    notificationInterval = setInterval(loadUnreadCount, 30000);
}

async function loadUnreadCount() {
    if (!isAuthenticated()) return;

    try {
        const response = await apiRequest('/notifications/unread-count');
        const badge = document.getElementById('notificationBadge');
        if (badge) {
            badge.textContent = response.count;
            badge.style.display = response.count > 0 ? 'flex' : 'none';
        }
    } catch (error) {
        console.error('Error loading unread count:', error);
    }
}

async function loadNotifications() {
    if (!isAuthenticated()) return;

    try {
        const notifications = await apiRequest('/notifications');
        displayNotifications(notifications);
    } catch (error) {
        console.error('Error loading notifications:', error);
    }
}

function displayNotifications(notifications) {
    const notificationList = document.getElementById('notificationList');
    
    if (notifications.length === 0) {
        notificationList.innerHTML = '<p class="no-notifications">Sem notificações</p>';
        return;
    }

    notificationList.innerHTML = notifications.map(notification => `
        <div class="notification-item ${notification.isRead ? '' : 'unread'}" data-notification-id="${notification.notificationId}" data-event-id="${notification.eventId || ''}">
            <h4>${escapeHtml(notification.title)}</h4>
            <p>${escapeHtml(notification.message)}</p>
            <p class="time">${formatTimeAgo(notification.createdAt)}</p>
        </div>
    `).join('');

    // Add click event listeners to each notification item
    notificationList.querySelectorAll('.notification-item').forEach(item => {
        item.addEventListener('click', async (e) => {
            e.stopPropagation(); // Prevent dropdown from closing
            const notificationId = parseInt(item.getAttribute('data-notification-id'));
            const eventId = item.getAttribute('data-event-id');
            
            // Mark as read
            await markNotificationAsRead(notificationId);
            
            // Navigate to event if available
            if (eventId) {
                window.location.href = `event-detail.html?id=${eventId}`;
            }
        });
    });
}

async function markNotificationAsRead(notificationId) {
    try {
        await apiRequest(`/notifications/${notificationId}/read`, { method: 'PUT' });
        loadNotifications();
        loadUnreadCount();
    } catch (error) {
        console.error('Error marking notification as read:', error);
    }
}

// Make function globally available for backwards compatibility
window.markNotificationAsRead = markNotificationAsRead;

async function markAllNotificationsAsRead() {
    try {
        await apiRequest('/notifications/read-all', { method: 'PUT' });
        loadNotifications();
        loadUnreadCount();
    } catch (error) {
        console.error('Error marking all notifications as read:', error);
    }
}

// Analytics Tracking
async function trackPageView(pageUrl) {
    const user = getCurrentUser();
    try {
        await apiRequest('/analytics/track', {
            method: 'POST',
            body: JSON.stringify({
                pageUrl: pageUrl,
                userId: user ? user.userId : null
            })
        });
    } catch (error) {
        console.error('Error tracking page view:', error);
    }
}

// Utility Functions
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function formatTimeAgo(dateString) {
    const date = new Date(dateString);
    const now = new Date();
    const seconds = Math.floor((now - date) / 1000);

    if (seconds < 60) return 'Agora mesmo';
    if (seconds < 3600) {
        const minutes = Math.floor(seconds / 60);
        return `${minutes} ${minutes === 1 ? 'minuto' : 'minutos'} atrás`;
    }
    if (seconds < 86400) {
        const hours = Math.floor(seconds / 3600);
        return `${hours} ${hours === 1 ? 'hora' : 'horas'} atrás`;
    }
    if (seconds < 604800) {
        const days = Math.floor(seconds / 86400);
        return `${days} ${days === 1 ? 'dia' : 'dias'} atrás`;
    }
    return date.toLocaleDateString('pt-BR');
}

function showError(message, elementId = 'errorMessage') {
    const errorElement = document.getElementById(elementId);
    if (errorElement) {
        errorElement.textContent = message;
        errorElement.style.display = 'block';
        setTimeout(() => {
            errorElement.style.display = 'none';
        }, 5000);
    } else {
        alert(message);
    }
}

function showSuccess(message, elementId = 'successMessage') {
    const successElement = document.getElementById(elementId);
    if (successElement) {
        successElement.textContent = message;
        successElement.style.display = 'block';
        setTimeout(() => {
            successElement.style.display = 'none';
        }, 5000);
    } else {
        alert(message);
    }
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
    initializeNav();
});

// Cleanup on page unload
window.addEventListener('beforeunload', () => {
    if (notificationInterval) {
        clearInterval(notificationInterval);
    }
});
