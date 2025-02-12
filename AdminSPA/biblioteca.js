document.addEventListener("DOMContentLoaded", function() {
    const loginModal = document.getElementById("loginModal");
    const overlay = document.getElementById("overlay");
    const loginForm = document.getElementById("loginForm");
    const welcomeUser = document.getElementById("welcomeUser");
    const loginButton = document.getElementById("loginButton");

    // Verifica se o usuário já está logado
    if (localStorage.getItem("usuarioLogado")) {
        liberarAcesso();
    } else {
        loginModal.style.display = "block";
        overlay.style.display = "block";
    }

    loginForm.addEventListener("submit", function(event) {
        event.preventDefault();
        const username = document.getElementById("username").value;
        const password = document.getElementById("password").value;

        // Aqui você pode verificar o usuário e senha com um banco de dados ou API externa.
        if (username === "admin" && password === "1234") {
            localStorage.setItem("usuarioLogado", username);
            liberarAcesso();
        } else {
            alert("Usuário ou senha incorretos!");
        }
    });

    function liberarAcesso() {
        loginModal.style.display = "none";
        overlay.style.display = "none";
        welcomeUser.innerText = "Bem-vindo, " + localStorage.getItem("usuarioLogado") + "!";
        welcomeUser.style.display = "inline";
        loginButton.style.display = "none";
    }

    // Para permitir logout
    function logout() {
        localStorage.removeItem("usuarioLogado");
        location.reload(); // Recarrega a página para exigir login novamente
    }

    // Adiciona um botão de logout opcional
    let logoutButton = document.createElement("button");
    logoutButton.innerText = "Sair";
    logoutButton.style.marginLeft = "10px";
    logoutButton.onclick = logout;
    welcomeUser.appendChild(logoutButton);
});
