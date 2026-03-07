// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.querySelectorAll(".ajax-add-form").forEach((form) => {
  form.addEventListener("submit", async function (e) {
    e.preventDefault();

    const btn = this.querySelector("button");
    const originalHtml = btn.innerHTML;

    btn.innerHTML = "<i class='fa-solid fa-spinner fa-spin'></i>";
    btn.disabled = true;

    try {
      const response = await fetch(this.action, {
        method: "POST",
        body: new FormData(this),
      });

      const result = await response.json();

      if (result.success) {
        btn.innerHTML = "<i class='fa-solid fa-check'></i> Added";
        btn.classList.remove("btn-primary", "btn-outline-danger");
        btn.classList.add("btn-success");

        setTimeout(() => {
          btn.className = originalHtml.includes("heart")
            ? "btn btn-danger"
            : "btn btn-primary";
          btn.disabled = false;
          btn.innerHTML = originalHtml.includes("heart")
            ? '<i class="fa fa-heart" aria-hidden="true"></i>'
            : originalHtml;
        }, 2000);
      }
    } catch (error) {
      console.error("Error:", error);
      btn.innerHTML = "Error";
    }
  });
});
