function isValidEmail(email) {
    // ✅ Regex for valid email format
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailPattern.test(email);
}

document.getElementById('registerForm').addEventListener('submit', function (event) {
    event.preventDefault();

    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value.trim();

    // ✅ Check if email is valid
    if (!isValidEmail(email)) {
        alert("❌ Please enter a valid email (e.g., user@example.com).");
        return;
    }

    // ✅ Create User in Firebase Auth
    auth.createUserWithEmailAndPassword(email, password)
        .then((userCredential) => {
            userCredential.user.sendEmailVerification()
                .then(() => {
                    alert('✅ Registration successful! Please check your email for verification.');
                    window.location.href = '/Home/Index';
                })
                .catch((error) => {
                    alert('❌ Error sending verification email: ' + error.message);
                });
        })
        .catch((error) => {
            // ✅ Handle Firebase errors
            if (error.code === "auth/invalid-email") {
                alert("❌ Invalid email format. Please enter a correct email.");
            } else if (error.code === "auth/email-already-in-use") {
                alert("⚠️ This email is already registered. Try logging in instead.");
            } else if (error.code === "auth/weak-password") {
                alert("⚠️ Password is too weak. Use at least 6 characters.");
            } else {
                alert("❌ Registration failed: " + error.message);
            }
        });
});