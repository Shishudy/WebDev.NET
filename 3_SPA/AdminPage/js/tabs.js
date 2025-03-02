function search() {
	let data = document.getElementById('search-box-input').value;
	console.log(data);
}

function changeTab(i) {
	if (i == 1)
		reservas();
	else if (i == 2)
		obras();
	else if (i == 3)
		nucleos();
	else if (i == 4)
		gestaoDeUsers();
}

function reservas() {
	
}

function obras() {
	
}

function nucleos() {
	
}

function gestaoDeUsers() {
	
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