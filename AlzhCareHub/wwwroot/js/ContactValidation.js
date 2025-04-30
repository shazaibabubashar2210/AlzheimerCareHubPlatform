$(document).ready(function () {
        $('#contactForm').on('submit', function (e) {
            var firstName = $('input[name="FirstName"]').val().trim();
            var lastName = $('input[name="LastName"]').val().trim();
            var email = $('input[name="Email"]').val().trim();
            var phoneNumber = $('input[name="PhoneNumber"]').val().trim();
            var message = $('textarea[name="Message"]').val().trim();

            var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            var phoneRegex = /^\+?\d{10,15}$/;

            if (firstName.length < 2 || /\d/.test(firstName)) {
                alert("Please enter a valid First Name (at least 2 letters, no numbers).");
                e.preventDefault();
                return;
            }
            if (lastName.length < 2 || /\d/.test(lastName)) {
                alert("Please enter a valid Last Name (at least 2 letters, no numbers).");
                e.preventDefault();
                return;
            }
            if (!emailRegex.test(email)) {
                alert("Please enter a valid Email address.");
                e.preventDefault();
                return;
            }
            if (!phoneRegex.test(phoneNumber)) {
                alert("Please enter a valid Phone Number (10-15 digits, optional + sign).");
                e.preventDefault();
                return;
            }
            if (message.length < 10) {
                alert("Please enter a Message with at least 10 characters.");
                e.preventDefault();
                return;
            }
        });
        });
