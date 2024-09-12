// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$("#showModalBtn").on("click", function () {
  $("#exampleModal").modal("show");
});

$("#closeBtn").on("click", function () {
  $("#exampleModal").modal("toggle");
});

$("#MaincloseBtn").on("click", function () {
  $("#exampleModal").modal("toggle");
});
//////////////////////// Thử Nghiệm //////////////////////////////
//$(document).ready(function () {
//    alert();
//})
$("#txtSearch").keyup(function () {
  var typeValue = $(this).val();
  $("tbody tr").each(function () {
    if ($(this).text().search(new RegExp(typeValue, "i")) < 0) {
      $(this).fadeOut();
    } else {
      $(this).show();
    }
  });
});
