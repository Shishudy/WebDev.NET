// const loginButton = document.getElementById("loginButton");
// const loginModal = document.getElementById("loginModal");
// const loginForm = document.getElementById("loginForm");
// const loginUsername = document.getElementById("username");
// const loginPassword = document.getElementById("password");
// const welcomeUser = document.getElementById("welcomeUser");
// const searchIcon = document.getElementById("pesquisa");
// const searchInput = document.getElementById("buscador");

// loginForm.addEventListener("submit", login);
// searchIcon.addEventListener("submit", searchInDB);

// window.onclick = function (event) {
// 	if (event.target == loginModal) {
// 		closeLoginModal();
// 	}
// };

// function openLoginModal() {
// 	loginModal.style.display = "block";
// }

// function closeLoginModal() {
// 	loginUsername.value = "";
// 	loginPassword.value = "";
// 	loginModal.style.display = "none";
// }

// function authentication() {
// 	if (utilizador.getUsername === undefined) {
// 		openLoginModal();
// 		return false;
// 	}
// 	return true;
// }

// function afterLogin() {
// 	loginButton.style.display = "none";
// 	welcomeUser.style.display = "block";
// 	welcomeUser.textContent = "Bem-vindo, " + utilizador.getUsername + "!";
// }

// function login(event) {
// 	event.preventDefault();
// 	if (loginUsername.value == "") {
// 		showPopup("Por favor, insira um nome de usuário!");
// 		return;
// 	}
// 	if (loginPassword.value.length < 4) {
// 		showPopup("A senha deve ter pelo menos 4 caracteres!");
// 		loginPassword.value = "";
// 		return;
// 	}
// 	utilizador = new User({ username: loginUsername.value, password: loginPassword.value });
// 	showPopup("Login realizado com sucesso!");
// 	closeLoginModal();
// 	afterLogin();
// 	buildPage();
// }

// function searchInDB(event) {
// 	event.preventDefault();
// 	cardListArray = db.filter(obj => Object.values(obj).some(value => String(value).toLowerCase().includes(searchInput.value.toLowerCase())));
// 	createCardListArray(cardListArray);
// 	clearPage();
// 	buildCatSelector();
// 	buildCardList();
// 	const bookList = document.getElementById("lista-livros");
// 	bookList.firstElementChild.textContent = "Pesquisa por: " + searchInput.value;
// 	searchInput.value = "";
// }


// Sample datadocument.addEventListener("DOMContentLoaded", function() {
  // Sample data
document.addEventListener("DOMContentLoaded", function() {
  // Sample data
  const data = [
    { "nomeObra": "1984", "timesRequested": 6 },
    { "nomeObra": "The Great Gatsby", "timesRequested": 4 },
    { "nomeObra": "To Kill a Mockingbird", "timesRequested": 4 },
    { "nomeObra": "The Hobbit", "timesRequested": 4 },
    { "nomeObra": "The Odyssey", "timesRequested": 4 },
    { "nomeObra": "Moby-Dick", "timesRequested": 3 },
    { "nomeObra": "War and Peace", "timesRequested": 3 },
    { "nomeObra": "Pride and Prejudice", "timesRequested": 3 },
    { "nomeObra": "The Catcher in the Rye", "timesRequested": 3 },
    { "nomeObra": "The Divine Comedy", "timesRequested": 3 }
  ];

  // Function to populate the table
  function populateTable(data) {
    const table = document.querySelector("#ShowTable");

    // Clear existing content (if any)
    table.innerHTML = '';

    // Create table headers dynamically
    const headers = Object.keys(data[0]); // Get keys from the first object in the data array
    const tableHeader = document.createElement('thead');
    const tableHeaderRow = document.createElement('tr');
    headers.forEach(header => {
      const th = document.createElement('th');
      th.textContent = header; // Set header text to the key
      tableHeaderRow.appendChild(th);
    });
    tableHeader.appendChild(tableHeaderRow);
    table.appendChild(tableHeader);

    // Create table body dynamically
    const tableBody = document.createElement('tbody');
    data.forEach(item => {
      const row = document.createElement('tr');
      headers.forEach(header => {
        const td = document.createElement('td');
        td.textContent = item[header]; // Populate cell with corresponding value from the data
        row.appendChild(td);
      });
	//   row.addEventListener('click', function() {
    //     // Highlight the selected row
    //     document.querySelectorAll('#ShowTable tbody tr').forEach(tr => tr.classList.remove('selected'));
    //     row.classList.add('selected');

    //     // Send selected row data to web API
    //     sendDataToAPI(item);
    //   });
      tableBody.appendChild(row);
    });
    table.appendChild(tableBody);
  }

  // Call the function to populate the table with data
  populateTable(data);
});