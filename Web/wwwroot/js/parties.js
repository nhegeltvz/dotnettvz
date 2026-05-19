const tableBody = document.getElementById("partiesTable");
const toast = document.querySelector(".dashboard-toast");
let toastTimeoutId = null;
document.addEventListener("DOMContentLoaded", async () => {
  await loadParties();

  document
    .querySelector("[data-autocomplete-root]")
    ?.addEventListener("autocomplete:confirm", (e) =>
      loadParties(e.detail.term),
    );
});

tableBody.addEventListener("click", function (e) {
  if (e.target.classList.contains("dashboard-row-button-edit")) {
    const party = JSON.parse(e.target.dataset.party);
    editParty(party);
  }
});

//injeted form actions
function fetchForm(callback) {
  dashboardSpinner.show();
  $.get("/parties/form", function (html) {
    $("#form-container").html(html);
    $.validator.unobtrusive.parse("#party-form");
    if (callback) callback();
  }).always(function () {
    dashboardSpinner.hide();
  });
}

function cancelForm() {
  $("#form-container").html("");
}

function setEditState(isEditing, label) {
  const form = document.getElementById("party-form");
  const banner = document.getElementById("party-edit-banner");

  if (!form || !banner) return;

  form.classList.toggle("dashboard-form--editing", isEditing);
  banner.hidden = !isEditing;
  banner.textContent = isEditing ? `Editing party: ${label || "Selected"}` : "";
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
function loadParties(search = "") {
  dashboardSpinner.show();
  const url = search
    ? `/parties/data?search=${encodeURIComponent(search)}`
    : "/parties/data";
  $.ajax({
    url,
    method: "GET",
    success: function (parties) {
      let injectedHtml = "";
      parties.forEach((party) => {
        injectedHtml += `
						<tr>
							<td>${party.playerCreatedUsername}</td>
							<td>${party.dateCreated}</td>
							<td>${party.partyDescription}</td>
							<td>${party.numberOfMembers}/${party.maxMembers}</td>
							<td>
								<button class="dashboard-row-button dashboard-row-button--edit dashboard-row-button-edit" data-party='${JSON.stringify(party)}'>Edit</button>
								<button class="dashboard-row-button dashboard-row-button--danger" onclick="deleteParty('${party.id}')">Delete</button>
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
    $("#party-id").val("");
    setEditState(false);
  });
}

function submitForm() {
  const id = $("#party-id").val();
  const url = id ? `/parties/edit/${id}` : "/parties/create";
  const method = "POST";

  if (!$("#party-form").valid()) return;

  $.ajax({
    url: url,
    method: method,
    contentType: "application/json",
    data: JSON.stringify({
      id: $("#party-id").val() || null,
      playerCreatedId: $("#Form_PlayerCreatedId").val(),
      dateCreated: $("#Form_DateCreated").val(),
      maxMembers: $("#Form_MaxMembers").val(),
      partyDescription: $("#Form_PartyDescription").val(),
      preferredLocations: $("#Form_PreferredLocations").val(),
    }),
    beforeSend: function () {
      dashboardSpinner.show();
    },
    success: function () {
      cancelForm();
      loadParties();
      showToast(id ? "Updated!" : "Saved!");
    },
    error: function (xhr) {
      renderValidationSummary(
        "party-form",
        collectValidationMessages(xhr.responseJSON),
      );
    },
    complete: function () {
      dashboardSpinner.hide();
    },
  });
}

//Delete
function deleteParty(id) {
  if (!confirm("Are you sure?")) return;

  $.ajax({
    url: `/parties/delete/${id}`,
    method: "DELETE",
    beforeSend: function () {
      dashboardSpinner.show();
    },
    success: function () {
      loadParties();
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
function editParty(party) {
  fetchForm(() => {
    $("#party-id").val(party.id);
    $("#Form_PlayerCreatedId").val(party.playerCreatedId);
    $("#Form_DateCreated").val(party.dateCreated);
    $("#Form_MaxMembers").val(party.maxMembers);
    $("#Form_PartyDescription").val(party.partyDescription);
    $("#Form_PreferredLocations").val(party.preferredLocations);
    setEditState(true, party.partyDescription);
  });
}
