let partyMembersSelect = null;
let preferredDates = [];
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
function fetchForm(partyId, callback) {
  dashboardSpinner.show();
  const url = partyId ? `/parties/form?id=${partyId}` : "/parties/form";
  $.get(url, function (html) {
    $("#form-container").html(html);
    $.validator.unobtrusive.parse("#party-form");
    partyMembersSelect = new HegelMultiSelect("hms-player-select");
    initPreferredDates();
    initAttendanceSection();
    if (window.initDatetimePickers) {
      initDatetimePickers(document.getElementById("party-form"));
    }
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
  fetchForm(null, () => {
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
      memberIds: partyMembersSelect ? partyMembersSelect.getData().ids : [],
      preferredPlayingDates: preferredDates,
      scheduledMatchId: $("#scheduled-match-id").val() || null,
      scheduledMatchPlayingFieldId:
        $("#Form_ScheduledMatchPlayingFieldId").val() || null,
      scheduledMatchDate: $("#Form_ScheduledMatchDate").val() || null,
      scheduledMatchAttendances: collectAttendances(),
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
      showErrorModal(collectValidationMessages(xhr.responseJSON));
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
  fetchForm(party.id, () => {
    $("#party-id").val(party.id);
    $("#Form_PlayerCreatedId").val(party.playerCreatedId);
    $("#Form_DateCreated").val(party.dateCreated);
    $("#Form_MaxMembers").val(party.maxMembers);
    $("#Form_PartyDescription").val(party.partyDescription);
    $("#Form_PreferredLocations").val(party.preferredLocations);
    setEditState(true, party.partyDescription);
  });
}

function initPreferredDates() {
  preferredDates = [];
  const list = document.getElementById("preferred-dates-list");
  const addButton = document.getElementById("add-preferred-date");

  if (!list) return;

  list.querySelectorAll(".dashboard-date-chip").forEach((chip) => {
    const dateValue = chip.dataset.date;
    if (dateValue) {
      preferredDates.push(dateValue);
    }
  });

  list.addEventListener("click", (event) => {
    if (!event.target.classList.contains("dashboard-chip-remove")) return;
    const chip = event.target.closest(".dashboard-date-chip");
    if (!chip) return;
    const dateValue = chip.dataset.date;
    if (dateValue) {
      preferredDates = preferredDates.filter((date) => date !== dateValue);
    }
    chip.remove();
  });

  if (addButton) {
    addButton.addEventListener("click", () => addPreferredDate(list));
  }
}

function addPreferredDate(list) {
  const input = document.getElementById("preferred-date-input");
  if (!input || !input.value) return;

  const dateValue = input.value;
  if (preferredDates.includes(dateValue)) return;

  preferredDates.push(dateValue);

  const chip = document.createElement("div");
  chip.className = "dashboard-date-chip";
  chip.dataset.date = dateValue;
  chip.innerHTML = `
    <span>${formatPreferredDate(dateValue)}</span>
    <button type="button" class="dashboard-chip-remove" aria-label="Remove date">X</button>
  `;

  list.appendChild(chip);
  input.value = "";
}

function formatPreferredDate(dateValue) {
  const parsed = new Date(dateValue);
  if (Number.isNaN(parsed.getTime())) return dateValue;

  return parsed.toLocaleString(undefined, {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function initAttendanceSection() {
  const section = document.getElementById("scheduled-attendance-section");
  if (!section) return;

    const rows = section.querySelectorAll("tbody tr");
    if (!rows.length) section.setAttribute("hidden", "hidden");

    document.getElementById("hms-player-select")?.addEventListener("hms:change", () => {
        refreshAttendanceRows(section);
    });

    document.getElementById("Form_ScheduledMatchPlayingFieldId")
        ?.addEventListener("change", () => refreshAttendanceRows(section));
    document.getElementById("Form_ScheduledMatchDate")
        ?.addEventListener("change", () => refreshAttendanceRows(section));
}

function refreshAttendanceRows(section) {
    const hasField = !!document.getElementById("Form_ScheduledMatchPlayingFieldId")?.value;
    const hasDate = !!document.getElementById("Form_ScheduledMatchDate")?.value;

    // selectedItems is { [id]: displayName } — getData() has no names, only ids
    const selected = partyMembersSelect ? partyMembersSelect.selectedItems : {};
    const members = Object.entries(selected); // [[id, displayName], ...]

    if (!hasField || !hasDate || !members.length) {
        section.setAttribute("hidden", "hidden");
        return;
    }

    const tbody = section.querySelector("tbody");
    const incomingIds = new Set(members.map(([id]) => id));

    // remove rows for deselected members first
    tbody.querySelectorAll("tr").forEach(row => {
        if (!incomingIds.has(row.dataset.playerId)) row.remove();
    });

    // compute what's left after removal, then add only truly new members
    const existingIds = new Set([...tbody.querySelectorAll("tr")].map(r => r.dataset.playerId));
    members.forEach(([id, name]) => {
        if (existingIds.has(id)) return;
        const tr = document.createElement("tr");
        tr.dataset.playerId = id;
        tr.dataset.attendanceId = "";
        tr.innerHTML = `<td>${name}</td><td><input type="checkbox" class="attendance-toggle" /></td>`;
        tbody.appendChild(tr);
    });

    section.removeAttribute("hidden");
}

function collectAttendances() {
  const section = document.getElementById("scheduled-attendance-section");
  if (!section || section.hasAttribute("hidden")) return [];

  const attendances = [];
  section.querySelectorAll("tbody tr").forEach((row) => {
    const playerId = row.dataset.playerId;
    const attendanceId = row.dataset.attendanceId || null;
    const checkbox = row.querySelector(".attendance-toggle");
    if (!playerId || !checkbox) return;

    attendances.push({
      id: attendanceId,
      playerId: playerId,
      isAttending: checkbox.checked,
    });
  });

  return attendances;
}
