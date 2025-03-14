document.addEventListener("DOMContentLoaded", function () {
    document.addEventListener("click", function (event) {
        if (event.target.classList.contains("check-availability")) {
            var doctorName = event.target.getAttribute("data-doctor");
            document.getElementById("doctorName").value = doctorName;
        }
    });

    document.getElementById("appointmentForm").addEventListener("submit", async function (event) {
        event.preventDefault();

        const doctor = document.getElementById("doctorName").value;
        const caregiverEmail = document.getElementById("caregiverEmail").value; // Match C# property
        const suggestedDate = document.getElementById("appointmentDate").value; // Match C# property
        const suggestedTime = document.getElementById("appointmentTime").value; // Match C# property

        const appointment = { Doctor: doctor, CaregiverEmail: caregiverEmail, SuggestedDate: suggestedDate, SuggestedTime: suggestedTime };

        console.log(appointment); // Debugging

        try {
            const response = await fetch("/Appointment/Book", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(appointment)
            });

            if (response.ok) {
                alert("Appointment booked successfully! A confirmation link has been sent to the doctor.");
                location.reload();
            } else {
                const errorData = await response.json(); // Get error message from server
                alert(`Failed to book appointment: ${errorData.error || "Unknown error"}`);
            }
        } catch (error) {
            console.error("Error:", error);
            alert("Error booking appointment.");
        }
    });
});
