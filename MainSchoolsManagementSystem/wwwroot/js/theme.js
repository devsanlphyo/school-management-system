window.themeManager = {
    // Get the current theme ('light', 'dark', or 'system')
    getTheme: function () {
        return localStorage.getItem('hellotwo-theme') || 'system';
    },

    // Set theme: 'light', 'dark', or 'system'
    setTheme: function (theme) {
        localStorage.setItem('hellotwo-theme', theme);

        if (theme === 'light') {
            document.documentElement.setAttribute('data-theme', 'light');
        } else if (theme === 'dark') {
            document.documentElement.setAttribute('data-theme', 'dark');
        } else {
            document.documentElement.removeAttribute('data-theme');
        }
    },

    // Apply the theme to the document — called on page load
    applyTheme: function () {
        var theme = this.getTheme();

        if (theme === 'light') {
            document.documentElement.setAttribute('data-theme', 'light');
        } else if (theme === 'dark') {
            document.documentElement.setAttribute('data-theme', 'dark');
        } else {
            document.documentElement.removeAttribute('data-theme');
        }
    }
};

// Apply theme immediately on script load
themeManager.applyTheme();
