const authState = {
    token: localStorage.getItem("expense-token") || "",
    userEmail: localStorage.getItem("expense-user-email") || "",
    mode: "login"
};

const authElements = {
    authForm: document.getElementById("authForm"),
    showLoginBtn: document.getElementById("showLoginBtn"),
    showRegisterBtn: document.getElementById("showRegisterBtn"),
    authTitle: document.getElementById("authTitle"),
    authEmail: document.getElementById("authEmail"),
    authPassword: document.getElementById("authPassword"),
    authSubmitBtn: document.getElementById("authSubmitBtn"),
    messageBox: document.getElementById("messageBox"),
    currentUser: document.getElementById("currentUser"),
    tokenStatus: document.getElementById("tokenStatus")
};

authElements.authForm.addEventListener("submit", onSubmit);
authElements.showLoginBtn.addEventListener("click", () => setMode("login"));
authElements.showRegisterBtn.addEventListener("click", () => setMode("register"));

renderAuthState();
setMode("login");

if (authState.token) {
    loadProfile(true);
}

function setMode(mode) {
    authState.mode = mode;
    const isLogin = mode === "login";

    authElements.authTitle.textContent = isLogin ? "Sign in" : "Create account";
    authElements.authPassword.placeholder = isLogin ? "Your password" : "Create a password";
    authElements.authSubmitBtn.textContent = isLogin
        ? "Continue to purchases"
        : "Create account and enter";

    authElements.showLoginBtn.classList.toggle("active", isLogin);
    authElements.showRegisterBtn.classList.toggle("active", !isLogin);
    authElements.messageBox.hidden = true;
}

async function onSubmit(event) {
    event.preventDefault();

    const payload = {
        email: authElements.authEmail.value.trim(),
        password: authElements.authPassword.value
    };

    if (authState.mode === "register") {
        const registerResponse = await fetch("/auth/register", {
            method: "POST",
            headers: jsonHeaders(),
            body: JSON.stringify(payload)
        });

        if (!registerResponse.ok) {
            return showMessage(await readError(registerResponse), true);
        }
    }

    const loginResponse = await fetch("/auth/login", {
        method: "POST",
        headers: jsonHeaders(),
        body: JSON.stringify(payload)
    });

    if (!loginResponse.ok) {
        return showMessage(await readError(loginResponse), true);
    }

    const data = await loginResponse.json();
    authState.token = data.token;
    localStorage.setItem("expense-token", authState.token);

    await loadProfile(false);
    authElements.authForm.reset();
    showMessage(authState.mode === "login"
        ? "Signed in. Redirecting to purchases."
        : "Account created. Redirecting to purchases.");
    redirectToExpenses();
}

async function loadProfile(redirectWhenValid) {
    const response = await fetch("/auth/me", {
        headers: authHeaders()
    });

    if (!response.ok) {
        authState.token = "";
        authState.userEmail = "";
        localStorage.removeItem("expense-token");
        localStorage.removeItem("expense-user-email");
        renderAuthState();
        return;
    }

    const user = await response.json();
    authState.userEmail = user.email;
    localStorage.setItem("expense-user-email", authState.userEmail);
    renderAuthState();

    if (redirectWhenValid) {
        redirectToExpenses();
    }
}

function redirectToExpenses() {
    window.setTimeout(() => {
        window.location.replace("/expenses.html");
    }, 300);
}

function renderAuthState() {
    authElements.currentUser.textContent = authState.userEmail || "Guest";
    authElements.tokenStatus.textContent = authState.token ? "Active token" : "No token";
}

function authHeaders() {
    return {
        ...jsonHeaders(),
        Authorization: `Bearer ${authState.token}`
    };
}

function jsonHeaders() {
    return {
        "Content-Type": "application/json",
        Accept: "application/json"
    };
}

async function readError(response) {
    try {
        const data = await response.json();
        return data.message || `Request failed: ${response.status}`;
    } catch {
        return `Request failed: ${response.status}`;
    }
}

function showMessage(message, isError = false) {
    authElements.messageBox.hidden = false;
    authElements.messageBox.textContent = message;
    authElements.messageBox.className = `message-box ${isError ? "error" : "success"}`;
}
