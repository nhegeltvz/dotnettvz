const tableBody = document.getElementById("matchRecordsTable");
const toast = document.querySelector(".dashboard-toast");
let toastTimeoutId = null;
document.addEventListener("DOMContentLoaded", async () => {
  await loadMatchRecords();

  document
    .querySelector("[data-autocomplete-root]")
    ?.addEventListener("autocomplete:confirm", (e) =>
      loadMatchRecords(e.detail.term),
    );
});

tableBody.addEventListener("click", function (e) {
  if (e.target.classList.contains("dashboard-row-button-edit")) {
    const record = JSON.parse(e.target.dataset.record);
    editRecord(record);
  }
});

//injeted form actions
function fetchForm(callback) {
  dashboardSpinner.show();
  $.get("/matches/form", function (html) {
    $("#form-container").html(html);
    $.validator.unobtrusive.parse("#match-record-form");
    if (callback) callback();
  }).always(function () {
    dashboardSpinner.hide();
  });
}

function cancelForm() {
  $("#form-container").html("");
}

function setEditState(isEditing, label) {
  const form = document.getElementById("match-record-form");
  const banner = document.getElementById("match-record-edit-banner");

  if (!form || !banner) return;

  form.classList.toggle("dashboard-form--editing", isEditing);
  banner.hidden = !isEditing;
  banner.textContent = isEditing
    ? `Editing match record: ${label || "Selected"}`
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
function loadMatchRecords(search = "") {
  dashboardSpinner.show();
  const url = search
    ? `/matches/data?search=${encodeURIComponent(search)}`
    : "/matches/data";
  $.ajax({
    url,
    method: "GET",
    success: function (records) {
      let injectedHtml = "";
      records.forEach((record) => {
        injectedHtml += `
                        <tr>
                            <td>${record.goalsTeamA}</td>
                            <td>${record.goalsTeamB}</td>
                            <td>${record.playingFieldName}</td>
                            <td>${record.matchHeld}</td>
                            <td>${record.wasMatchHeld}</td>
                            <td>
                                <button class="dashboard-row-button dashboard-row-button--edit dashboard-row-button-edit" data-record='${JSON.stringify(record)}'>Edit</button>
                                <button class="dashboard-row-button dashboard-row-button--danger" onclick="deleteRecord('${record.id}')">Delete</button>
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
    $("#match-record-id").val("");
    setEditState(false);
  });
}

function submitForm() {
  const id = $("#match-record-id").val();
  const url = id ? `/matches/edit/${id}` : "/matches/create";
  const method = "POST";

  if (!$("#match-record-form").valid()) return;

  $.ajax({
    url: url,
    method: method,
    contentType: "application/json",
    data: JSON.stringify({
      id: $("#match-record-id").val() || null,
      wasMatchHeld: $("#Form_WasMatchHeld").is(":checked"),
      matchHeld: $("#Form_MatchHeld").val(),
      playingFieldId: $("#Form_PlayingFieldId").val(),
      goalsTeamA: $("#Form_GoalsTeamA").val(),
      goalsTeamB: $("#Form_GoalsTeamB").val(),
    }),
    beforeSend: function () {
      dashboardSpinner.show();
    },
    success: function () {
      cancelForm();
      loadMatchRecords();
      showToast(id ? "Updated!" : "Saved!");
    },
    error: function (xhr) {
      renderValidationSummary(
        "match-record-form",
        collectValidationMessages(xhr.responseJSON),
      );
    },
    complete: function () {
      dashboardSpinner.hide();
    },
  });
}

//Delete
function deleteRecord(id) {
  if (!confirm("Are you sure?")) return;

  $.ajax({
    url: `/matches/delete/${id}`,
    method: "DELETE",
    beforeSend: function () {
      dashboardSpinner.show();
    },
    success: function () {
      loadMatchRecords();
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
function editRecord(record) {
  fetchForm(() => {
    $("#match-record-id").val(record.id);
    $("#Form_WasMatchHeld").prop("checked", record.wasMatchHeld);
    $("#Form_MatchHeld").val(record.matchHeld);
    $("#Form_PlayingFieldId").val(record.playingFieldId);
    $("#Form_GoalsTeamA").val(record.goalsTeamA);
    $("#Form_GoalsTeamB").val(record.goalsTeamB);
    setEditState(true, record.playingFieldName);
  });
}
