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

        // fetch('https://example.com/protected-endpoint', {
        //     method: 'POST',
        //     headers: {
        //         "Authorization": "Bearer your-jwt-token",   // For authentication
        //         "Content-Type": "application/json"          // To indicate that you're sending JSON data
        //     },
        //     body: JSON.stringify({ key: 'value' })          // Data you're sending in the body
        // })
        // .then(response => response.json())
        // .then(data => console.log(data))
        // .catch(error => console.error("Error:", error));
        
        // fetch("http://localhost:5000/login", {
        //     method: "POST",
        //     headers: {
        //       "Content-Type": "application/json"
        //     },
        //     body: JSON.stringify({ login, password })
        //   })
        //     .then(response => response.json())
        //     .then(data => liberarAcesso())
        //     .catch(error => alert("Error:", error));
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
