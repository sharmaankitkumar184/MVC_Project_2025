
//Validation Code

document.addEventListener("DOMContentLoaded", function () {

    const summaries = document.querySelectorAll(
        ".validation-summary-errors, .validation-alert"
    );

    summaries.forEach(summary => {

        if (summary.innerText.trim().length > 0) {

            setTimeout(() => {
                summary.classList.add("fade-out");
            }, 2000);

            setTimeout(() => {
                summary.style.display = "none";
            }, 2500);

        }
    });
});

    setTimeout(function () {
        let alert = document.getElementById("successAlert");

    if(alert){
        alert.classList.add("fade");

        setTimeout(()=>{
        alert.remove();
        },500);
    }
},5000);