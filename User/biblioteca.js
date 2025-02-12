class User {
	#_username;
	#_password;

	constructor(data) {
		if (!data) return;
		this.#_username = data.username;
		this.#_password = data.password;
	}

	get getUsername() {
		return this.#_username;
	}

	get getPassword() {
		return this.#_password;
	}

	toJSON() {
		return JSON.stringify({
			username: this.getUsername,
			password: this.getPassword
		});
	}
}

let utilizador = new User();
let selectedCat = "";
let uniqueCats = ["Selecionar"];

function getUniqueCats() {
	const categorias = db.map((list) => list.categoria);
	uniqueCats.push(...new Set(categorias));
}

function clearFilter() {
	selectedCat = "";
	createCardListArray(Array.from(db));
	buildPage();
}

function clearPage() {
	const bookList = document.getElementById("lista-livros");
	if (bookList.childElementCount < 3) return;
	let toDelete = bookList.getElementsByClassName("select-cat-div");
	if (toDelete.length > 0) toDelete[0].parentNode.removeChild(toDelete[0]);
	toDelete = bookList.getElementsByClassName("row");
	while (toDelete[0]) toDelete[0].parentNode.removeChild(toDelete[0]);
	toDelete = bookList.getElementsByClassName("pagination");
	toDelete[0].parentNode.removeChild(toDelete[0]);
}

function changeHeader() {
	const bookList = document.getElementById("lista-livros");
	if (selectedCat == "") bookList.firstElementChild.textContent = "Livros";
	else
		bookList.firstElementChild.textContent =
			" Livros " + ` sobre ${selectedCat.charAt(0).toUpperCase() + selectedCat.slice(1)}`;
}

function buildPage() {
	clearPage();
	buildCatSelector();
	changeHeader();
}

function showPopup(message) {
	let popup = document.getElementById("popup");
	popup.innerHTML = message;
	popup.style.display = "block";
	setTimeout(() => {
		popup.style.display = "none";
	}, 2000);
}
