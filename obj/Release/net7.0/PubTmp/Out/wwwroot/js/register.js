document.addEventListener("DOMContentLoaded", function() {
    const inputs = document.querySelectorAll(".form-floating input, .form-floating select");

    inputs.forEach(input => {
        input.addEventListener("focus", function() {
            input.style.borderColor = "#007bff";
            input.style.boxShadow = "0 0 5px rgba(0, 123, 255, 0.5)";
        });

        input.addEventListener("blur", function() {
            input.style.borderColor = "#ced4da";
            input.style.boxShadow = "none";
        });
    });
});
