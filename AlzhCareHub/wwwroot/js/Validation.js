document.addEventListener("DOMContentLoaded", function () {
       var form = document.querySelector("form");

        form.addEventListener("submit", function (event) {
            let title = document.querySelector("[name='Title']").value;
            let email = document.querySelector("[name='Email']").value;
            let date = document.querySelector("[name='Date']").value;
            let time = document.querySelector("[name='Time']").value;
            let description = document.querySelector("[name='Description']").value;

            // Regular expressions for validation
            let emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

            let titlePattern = /^[a-zA-Z\s]+$/; // Only letters and spaces allowed
            let now = new Date();
            let selectedDateTime = new Date(date + "T" + time);

            // Title Validation
            if (!titlePattern.test(title)) {
                alert("Title should contain only letters and spaces. Numbers and special characters are not allowed.");
                event.preventDefault();
                return;
            }

            // Email Validation
            if (!emailPattern.test(email)) {
                alert("Please enter a valid email address.");
                event.preventDefault();
                return;
            }

            // Date & Time Validation (should be future)
            if (selectedDateTime <= now) {
                alert("The selected date and time must be in the future.");
                event.preventDefault();
                return;
            }

            // Description Validation (should not contain garbage characters)
            if (description.trim().length < 5) {
                alert("Please enter a meaningful task description (at least 5 characters).");
                event.preventDefault();
                return;
            }
        });
    });
