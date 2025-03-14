document.addEventListener("DOMContentLoaded", function () {
    // Booking Appointment
    document.addEventListener("click", function (event) {
        if (event.target.classList.contains("check-availability")) {
            var doctorName = event.target.getAttribute("data-doctor");
            document.getElementById("doctorName").value = doctorName;
        }
    });

    document.getElementById("appointmentForm").addEventListener("submit", async function (event) {
        event.preventDefault();

        const doctor = document.getElementById("doctorName").value;
        const caregiverEmail = document.getElementById("caregiverEmail").value;
        const suggestedDate = document.getElementById("appointmentDate").value;
        const suggestedTime = document.getElementById("appointmentTime").value;

        const appointment = { Doctor: doctor, CaregiverEmail: caregiverEmail, SuggestedDate: suggestedDate, SuggestedTime: suggestedTime };

        try {
            const response = await fetch("/Appointment/Book", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(appointment)
            });

            if (response.ok) {
                alert("Appointment booked successfully! Redirecting to the appointment list...");
                window.location.href = "/Appointment/Index"; // Redirects to Index page
            } else {
                const errorData = await response.json();
                alert(`Failed to book appointment: ${errorData.error || "Unknown error"}`);
            }
        } catch (error) {
            console.error("Error:", error);
            alert("Error booking appointment.");
        }
    });


    // Cancel Appointment
    document.querySelectorAll(".cancel-btn").forEach(button => {
        button.addEventListener("click", async function () {
            const appointmentId = this.getAttribute("data-id");

            if (confirm("Are you sure you want to cancel this appointment?")) {
                try {
                    const response = await fetch("/Appointment/CancelAppointment", {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify({ Id: appointmentId })
                    });

                    if (response.ok) {
                        alert("Appointment canceled successfully!");
                        location.reload();
                    } else {
                        alert("Failed to cancel the appointment.");
                    }
                } catch (error) {
                    console.error("Error:", error);
                }
            }
        });
    });

    // Reschedule Appointment - Open Modal
    document.querySelectorAll(".reschedule-btn").forEach(button => {
        button.addEventListener("click", function () {
            document.getElementById("rescheduleId").value = this.getAttribute("data-id");
        });
    });

    // Confirm Reschedule
    document.getElementById("confirmReschedule").addEventListener("click", async function () {
        const appointmentId = document.getElementById("rescheduleId").value;
        const newDate = document.getElementById("newDate").value;
        const newTime = document.getElementById("newTime").value;

        if (!newDate || !newTime) {
            alert("Please select a valid date and time.");
            return;
        }

        try {
            const response = await fetch("/Appointment/RescheduleAppointment", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Id: appointmentId, SuggestedDate: newDate, SuggestedTime: newTime })
            });

            if (response.ok) {
                alert("Reschedule request sent successfully!");
                location.reload();
            } else {
                alert("Failed to reschedule the appointment.");
            }
        } catch (error) {
            console.error("Error:", error);
        }
    });
});