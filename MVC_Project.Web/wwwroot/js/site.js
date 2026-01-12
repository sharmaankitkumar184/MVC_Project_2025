
//Validation Code

document.addEventListener("DOMContentLoaded", function () {

    const summary = document.querySelector(".validation-summary-errors");

    if (summary) {
        // Show for 2 seconds
        setTimeout(() => {
            summary.classList.add("fade-out");
        }, 2000);

        // Remove completely after fade animation
        setTimeout(() => {
            summary.style.display = "none";
        }, 2500);
    }
});
