const expenseState = {
    token: localStorage.getItem("expense-token") || "",
    userEmail: localStorage.getItem("expense-user-email") || "",
    expenses: []
};

if (!expenseState.token) {
    window.location.replace("/login.html");
}

const expenseElements = {
    expenseForm: document.getElementById("expenseForm"),
    logoutBtn: document.getElementById("logoutBtn"),
    refreshBtn: document.getElementById("refreshBtn"),
    cancelEditBtn: document.getElementById("cancelEditBtn"),
    expenseTableBody: document.getElementById("expenseTableBody"),
    messageBox: document.getElementById("messageBox"),
    currentUser: document.getElementById("currentUser"),
    tokenStatus: document.getElementById("tokenStatus"),
    expenseCount: document.getElementById("expenseCount"),
    expenseTotal: document.getElementById("expenseTotal"),
    expenseId: document.getElementById("expenseId"),
    amount: document.getElementById("amount"),
    category: document.getElementById("category"),
    description: document.getElementById("description"),
    date: document.getElementById("date"),
    saveBtn: document.getElementById("saveBtn")
};

expenseElements.expenseForm.addEventListener("submit", onSaveExpense);
expenseElements.logoutBtn.addEventListener("click", onLogout);
expenseElements.refreshBtn.addEventListener("click", () => loadExpenses(true));
expenseElements.cancelEditBtn.addEventListener("click", resetExpenseForm);

renderSession();
resetExpenseForm();
loadProfile().then(() => loadExpenses()).catch(() => {});

async function loadProfile() {
    const response = await fetch("/auth/me", {
        headers: authHeaders()
    });

    if (!response.ok) {
        onLogout(false);
        showMessage("Session expired. Please sign in again.", true);
        return;
    }

    const user = await response.json();
    expenseState.userEmail = user.email;
    localStorage.setItem("expense-user-email", expenseState.userEmail);
    renderSession();
}

async function loadExpenses(showToast = false) {
    const response = await fetch("/expenses", {
        headers: authHeaders()
    });

    if (!response.ok) {
        return showMessage(await readError(response), true);
    }

    expenseState.expenses = await response.json();
    renderExpenses(expenseState.expenses);

    if (showToast) {
        showMessage("Purchase list updated.");
    }
}

async function onSaveExpense(event) {
    event.preventDefault();

    const id = expenseElements.expenseId.value;
    const payload = {
        amount: Number(expenseElements.amount.value),
        category: expenseElements.category.value.trim(),
        description: expenseElements.description.value.trim(),
        date: new Date(expenseElements.date.value).toISOString()
    };

    const response = await fetch(id ? `/expenses/${id}` : "/expenses", {
        method: id ? "PUT" : "POST",
        headers: authHeaders(),
        body: JSON.stringify(payload)
    });

    if (!response.ok) {
        return showMessage(await readError(response), true);
    }

    resetExpenseForm();
    await loadExpenses();
    showMessage(id ? "Purchase updated." : "Purchase added.");
}

async function onDeleteExpense(id) {
    if (!window.confirm("Delete this purchase?")) {
        return;
    }

    const response = await fetch(`/expenses/${id}`, {
        method: "DELETE",
        headers: authHeaders()
    });

    if (!response.ok) {
        return showMessage(await readError(response), true);
    }

    if (expenseElements.expenseId.value === String(id)) {
        resetExpenseForm();
    }

    await loadExpenses();
    showMessage("Purchase deleted.");
}

function onEditExpense(expense) {
    expenseElements.expenseId.value = expense.id;
    expenseElements.amount.value = expense.amount;
    expenseElements.category.value = expense.category;
    expenseElements.description.value = expense.description;
    expenseElements.date.value = toDateTimeLocal(expense.date);
    expenseElements.saveBtn.textContent = "Update purchase";
    window.scrollTo({ top: 0, behavior: "smooth" });
}

function onLogout(showRedirect = true) {
    expenseState.token = "";
    expenseState.userEmail = "";
    expenseState.expenses = [];
    localStorage.removeItem("expense-token");
    localStorage.removeItem("expense-user-email");
    if (showRedirect) {
        window.location.replace("/login.html");
    }
}

function resetExpenseForm() {
    expenseElements.expenseForm.reset();
    expenseElements.expenseId.value = "";
    expenseElements.date.value = toDateTimeLocal(new Date());
    expenseElements.saveBtn.textContent = "Save purchase";
}

function renderSession() {
    expenseElements.currentUser.textContent = expenseState.userEmail || "Guest";
    expenseElements.tokenStatus.textContent = expenseState.token ? "Active token" : "No token";
}

function renderExpenses(expenses) {
    expenseElements.expenseCount.textContent = String(expenses.length);
    expenseElements.expenseTotal.textContent = formatMoney(
        expenses.reduce((sum, expense) => sum + Number(expense.amount), 0)
    );

    if (!expenses.length) {
        expenseElements.expenseTableBody.innerHTML = `
            <tr>
                <td colspan="5" class="empty-state">No purchases yet. Add the first one.</td>
            </tr>
        `;
        return;
    }

    expenseElements.expenseTableBody.innerHTML = expenses
        .map(expense => `
            <tr>
                <td>${formatDate(expense.date)}</td>
                <td>${escapeHtml(expense.category)}</td>
                <td>${escapeHtml(expense.description)}</td>
                <td>${formatMoney(expense.amount)}</td>
                <td>
                    <div class="row-actions">
                        <button class="edit-btn" type="button" data-action="edit" data-id="${expense.id}">Edit</button>
                        <button class="delete-btn" type="button" data-action="delete" data-id="${expense.id}">Delete</button>
                    </div>
                </td>
            </tr>
        `)
        .join("");

    expenseElements.expenseTableBody.querySelectorAll("button[data-action='edit']").forEach(button => {
        button.addEventListener("click", () => {
            const expense = expenses.find(item => item.id === Number(button.dataset.id));
            if (expense) {
                onEditExpense(expense);
            }
        });
    });

    expenseElements.expenseTableBody.querySelectorAll("button[data-action='delete']").forEach(button => {
        button.addEventListener("click", () => onDeleteExpense(Number(button.dataset.id)));
    });
}

function showMessage(message, isError = false) {
    expenseElements.messageBox.hidden = false;
    expenseElements.messageBox.textContent = message;
    expenseElements.messageBox.className = `message-box ${isError ? "error" : "success"}`;
}

async function readError(response) {
    try {
        const data = await response.json();
        return data.message || `Request failed: ${response.status}`;
    } catch {
        return `Request failed: ${response.status}`;
    }
}

function authHeaders() {
    return {
        "Content-Type": "application/json",
        Accept: "application/json",
        Authorization: `Bearer ${expenseState.token}`
    };
}

function formatMoney(value) {
    return `${Number(value).toFixed(2)} RUB`;
}

function formatDate(value) {
    return new Date(value).toLocaleString();
}

function toDateTimeLocal(value) {
    const date = new Date(value);
    const offset = date.getTimezoneOffset();
    const local = new Date(date.getTime() - offset * 60000);
    return local.toISOString().slice(0, 16);
}

function escapeHtml(value) {
    return value
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#39;");
}
