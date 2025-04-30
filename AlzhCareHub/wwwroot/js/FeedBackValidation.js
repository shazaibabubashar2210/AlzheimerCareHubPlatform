document.getElementById('feedbackForm').addEventListener('submit', function (e) {
    var email = document.getElementById('UserEmail').value.trim();
    var message = document.getElementById('Message').value.trim();

    // Regular Expression for simple email format
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!emailRegex.test(email)) {
        e.preventDefault();
        Swal.fire({
            icon: 'error',
            title: 'Invalid Email',
            text: 'Please enter a valid email address.'
        });
        return;
    }

    if (message.length < 10) {
        e.preventDefault();
        Swal.fire({
            icon: 'warning',
            title: 'Message too short',
            text: 'Please write a message of at least 10 characters.'
        });
        return;
    }
});