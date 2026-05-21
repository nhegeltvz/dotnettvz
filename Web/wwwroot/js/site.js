function showErrorModal(messages) {
    if (!messages || !messages.length) return;

    const body = document.getElementById("error-modal-body");
    if (!body) return;

    body.innerHTML = messages
        .map((m) => `<p class="mb-1">${m}</p>`)
        .join("");

    const modalEl = document.getElementById("error-modal");
    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    modal.show();

    setTimeout(() => modal.hide(), 4000);
}
