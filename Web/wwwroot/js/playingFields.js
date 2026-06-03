const tableBody = document.getElementById("playingFieldTable");
const toast = document.querySelector(".dashboard-toast");
let toastTimeoutId = null;
document.addEventListener("DOMContentLoaded", async () => {
  await loadStadiums();

  document
    .querySelector("[data-autocomplete-root]")
    ?.addEventListener("autocomplete:confirm", (e) =>
      loadStadiums(e.detail.term),
    );
});

tableBody.addEventListener("click", function (e) {
  if (e.target.classList.contains("dashboard-row-button-edit")) {
    const stadium = JSON.parse(e.target.dataset.stadium);
    editStadium(stadium);
  }
});

//injeted form actions
function fetchForm(callback) {
  dashboardSpinner.show();
  $.get("/stadiums/form", function (html) {
    $("#form-container").html(html);
    $.validator.unobtrusive.parse("#stadium-form");

    if (callback) callback();
  }).always(function () {
    dashboardSpinner.hide();
  });
}

function cancelForm() {
  $("#form-container").html("");
}

function setEditState(isEditing, label) {
  const form = document.getElementById("stadium-form");
  const banner = document.getElementById("stadium-edit-banner");

  if (!form || !banner) return;

  form.classList.toggle("dashboard-form--editing", isEditing);
  banner.hidden = !isEditing;
  banner.textContent = isEditing
    ? `Editing playing field: ${label || "Selected"}`
    : "";
}

function showToast(message) {
  if (!toast) return;
  toast.textContent = message;
  toast.classList.add("is-show");
  if (toastTimeoutId) {
    window.clearTimeout(toastTimeoutId);
  }
  toastTimeoutId = window.setTimeout(
    () => toast.classList.remove("is-show"),
    1800,
  );
}

function collectValidationMessages(responseJson) {
  if (!responseJson) return [];

  if (Array.isArray(responseJson)) {
    return responseJson
      .map((error) => {
        if (!error) return null;
        if (typeof error === "string") return error;
        return error.description || error.Description || null;
      })
      .filter(Boolean);
  }

  if (responseJson.errors) {
    return Object.values(responseJson.errors).flat();
  }

  if (typeof responseJson === "string") {
    return [responseJson];
  }

  return [];
}

function renderValidationSummary(formId, messages) {
  const form = document.getElementById(formId);
  if (!form) return;

  const summary = form.querySelector("[data-valmsg-summary]");
  if (!summary) return;

  summary.classList.remove("validation-summary-valid");
  summary.classList.add("validation-summary-errors");
  summary.innerHTML = "";

  if (!messages.length) {
    summary.classList.add("validation-summary-valid");
    summary.classList.remove("validation-summary-errors");
    return;
  }

  const list = document.createElement("ul");
  messages.forEach((message) => {
    const item = document.createElement("li");
    item.textContent = message;
    list.appendChild(item);
  });
  summary.appendChild(list);
}

//Read
function loadStadiums(search = "") {
  dashboardSpinner.show();
  const url = search
    ? `/stadiums/data?search=${encodeURIComponent(search)}`
    : "/stadiums/data";
  $.ajax({
    url,
    method: "GET",
    success: function (stadiums) {
      let injectedHtml = "";
      stadiums.forEach((stadium) => {
        injectedHtml += `
					<tr>
						<td>${stadium.name}</td>
						<td>${stadium.surfaceType}</td>
						<td>${stadium.longitude}/${stadium.latitude}</td>
						<td>
							<button class="dashboard-row-button dashboard-row-button--edit dashboard-row-button-edit" data-stadium='${JSON.stringify(stadium)}'>Edit</button>
							<button class="dashboard-row-button dashboard-row-button--danger" onclick="deleteStadium('${stadium.id}')">Delete</button>
						</td>
					</tr>
					`;
      });
      tableBody.innerHTML = injectedHtml;
    },
    error: function () {
      return;
    },
    complete: function () {
      dashboardSpinner.hide();
    },
  });
}

//Create
function openCreate() {
  fetchForm(() => {
    $("#stadium-id").val("");
    setEditState(false);
  });
}

function submitForm() {
  const id = $("#stadium-id").val();
  const url = id ? `/stadiums/edit/${id}` : "/stadiums/create";
    const method = "POST";

    if (!$("#stadium-form").valid()) return;

  var imageIds = typeof window.getUploadedImageIds === "function" ? window.getUploadedImageIds() : [];

  $.ajax({
    url: url,
    method: method,
    contentType: "application/json",
    data: JSON.stringify({
      id: $("#stadium-id").val() || null,
      name: $("#Name").val(),
      description: $("#Description").val(),
      longitude: $("#Longitude").val(),
      latitude: $("#Latitude").val(),
      contactNumber: $("#ContactNumber").val(),
      status: $("#Status").val(),
      isOutdoor: $("#IsOutdoor").is(":checked"),
      surfaceType: $("#SurfaceType").val(),
      imageIds: imageIds,
    }),
      beforeSend: function () {
      dashboardSpinner.show();
    },
    success: function () {
      cancelForm();
      loadStadiums();
      showToast(id ? "Updated!" : "Saved!");
    },
    error: function (xhr) {
      showErrorModal(collectValidationMessages(xhr.responseJSON));
    },
    complete: function () {
      dashboardSpinner.hide();
    },
  });
}

//Delete
function deleteStadium(id) {
  if (!confirm("Are you sure?")) return;

  $.ajax({
    url: `/stadiums/delete/${id}`,
    method: "DELETE",
    beforeSend: function () {
      dashboardSpinner.show();
    },
    success: function () {
      loadStadiums();
      showToast("Deleted!");
    },
    error: function (xhr) {
      console.log(xhr.responseJSON);
    },
    complete: function () {
      dashboardSpinner.hide();
    },
  });
}

//Edit
function editStadium(stadium) {
  fetchForm(() => {
    $("#stadium-id").val(stadium.id);
    $("#Name").val(stadium.name);
    $("#Description").val(stadium.description);
    $("#ContactNumber").val(stadium.contactNumber);
    $("#Status").val(stadium.status);
    $("#SurfaceType").val(stadium.surfaceType);
    $("#IsOutdoor").prop("checked", stadium.isOutdoor);
    $("#Latitude").val(stadium.latitude);
    $("#Longitude").val(stadium.longitude);

    setEditState(true, stadium.name);

    $.getJSON("/stadiums/" + stadium.id + "/images", function (images) {
      if (images && images.length > 0 && typeof window.preloadDropzoneImages === "function") {
        window.preloadDropzoneImages(images);
      }
    });
  });
}
