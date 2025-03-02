const apiUrl = "http://localhost:5164/";

document.addEventListener("DOMContentLoaded", function () {
	document.getElementById("login-modal-div").style.display = "flex";
});

document.body.addEventListener("submit", function (event) {
	event.preventDefault();
	if (event.target.id === "login-form")
		login(event.target.elements);
	else if (event.target.id === "form-input")
		submitForm(event);
});

function search(data) {
	console.log(data);
}

function buildPagination() {
	const page = document.getElementById("lista-cursos");
	let pagination = document.createElement("div");
	pagination.setAttribute("id", "pagination");
	pagination.classList.add("pagination");
	let node = document.createElement("a");
	node.setAttribute("onclick", `changePage("<<");`);
	node.textContent = `<<`;
	pagination.appendChild(node);
	node = document.createElement("a");
	node.setAttribute("onclick", `changePage("<");`);
	node.textContent = `<`;
	pagination.appendChild(node);
	let startPage = Math.max(0, cardListIterator - Math.floor(5 / 2));
	let endPage = Math.min(cardListArray.length, startPage + 5);
	if (endPage - startPage <= 4) {
		startPage = Math.max(0, endPage - 5);
	}
	while (startPage < endPage) {
		node = document.createElement("a");
		node.setAttribute("href", "#lista-cursos");
		node.setAttribute("onclick", `changePage(${startPage});`);
		if (startPage == cardListIterator) {
			node.setAttribute("class", "active");
		}
		node.textContent = startPage + 1;
		pagination.appendChild(node);
		startPage++;
	}
	node = document.createElement("a");
	node.setAttribute("onclick", `changePage(">");`);
	node.textContent = `>`;
	pagination.appendChild(node);
	node = document.createElement("a");
	node.setAttribute("onclick", `changePage(">>");`);
	node.textContent = `>>`;
	pagination.appendChild(node);
	page.appendChild(pagination);
}