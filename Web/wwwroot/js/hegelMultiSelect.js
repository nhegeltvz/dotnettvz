class HegelMultiSelect {
  constructor(containerId, options = {}) {
    this.container = document.getElementById(containerId);
    this.fieldName = this.container.dataset.fieldName;
    this.availableItems = JSON.parse(this.container.dataset.available);
    this.maxVisible = options.maxVisible ?? 5;

    this.input = this.container.querySelector(".hms-search-input");
    this.dropdown = this.container.querySelector(".hms-dropdown");
    this.selectedList = this.container.querySelector(".hms-selected-list");
    this.hiddenInput = this.container.querySelector(".hms-hidden-value");

    this.selectedItems = this._loadPreselected();
    this._syncHidden();
    this._bindEvents();
  }

  _loadPreselected() {
    const items = {};
    this.selectedList.querySelectorAll("li").forEach((li) => {
      items[li.dataset.id] = li.querySelector("span").textContent;
    });
    return items;
  }

  _bindEvents() {
    this.input.addEventListener("input", () => {
      this._filter(this.input.value);
    });

    this.selectedList.addEventListener("click", (e) => {
      if (e.target.classList.contains("hms-remove-btn")) {
        console.log(e.target);
        const li = e.target.closest("li");
        delete this.selectedItems[li.dataset.id];
        li.remove();
        this._syncHidden();
        this._notifyChange();
      }
    });
  }

  _filter(query) {
    const q = query.trim().toLowerCase();

    const results = this.availableItems.filter(
      (item) =>
        !this.selectedItems[item.id] &&
        (q === "" ||
          item.displayName.toLowerCase().includes(q) ||
          item.id.toLowerCase().includes(q)),
    );

    this._renderDropdown(results);
  }

  _renderDropdown(results) {
    this.dropdown.innerHTML = "";
    const visible = results.slice(0, this.maxVisible);

    if (!results.length) {
      const li = document.createElement("li");
      li.className = "ms-no-results";
      li.textContent = "No results";
      this.dropdown.appendChild(li);
    } else {
      results.forEach((item) => {
        const li = document.createElement("li");
        li.textContent = item.displayName;
        li.addEventListener("click", () => this._selectItem(item));
        this.dropdown.appendChild(li);
      });
    }

    this.dropdown.style.display = "block";
  }

  _selectItem(item) {
    if (this.selectedItems[item.id]) return;

    this.selectedItems[item.id] = item.displayName;

    const li = document.createElement("li");
    li.dataset.id = item.id;
    li.innerHTML = `<span>${item.displayName}</span>
                        <button type="button" class="hms-remove-btn" aria-label="Remove">X</button>`;

    this.selectedList.appendChild(li);
    this._syncHidden();
    this._notifyChange();
    this.input.value = "";
    this.dropdown.style.display = "none";
  }

  _syncHidden() {
    this.hiddenInput.value = JSON.stringify(Object.keys(this.selectedItems));
  }

  _notifyChange() {
    this.container.dispatchEvent(
      new CustomEvent("hms:change", {
        detail: this.getData(),
      }),
    );
  }

  getData() {
    return {
      field: this.fieldName,
      ids: Object.keys(this.selectedItems),
    };
  }
}
