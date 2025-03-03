function search() {
	let data = document.getElementById("search-box-input").value;
	console.log(data);
}

function changeTab(tab) {
	document.getElementById("loading-modal-div").style.display = "flex";
	if (tab === "reservas")
		reservas();
	else if (tab === "obras")
		obras();
	else if (tab === "nucleos")
		nucleos();
	else if (tab === "gestao")
		gestaoDeUsers();
	// let tabMethods;
	// fetch(apiUrl + `methods/${tab}`).then((response) => {
	// 	if (!response.ok) {
	// 		throw new Error('Network response was not ok');
	// 	}
	// 	return response.json();
	// }).then((data) => {
	// 	tabMethods = data;
	// }).catch((error) => {
	// 	console.error("Error:", error);
	// });
	// //document.getElementById("tab-div").style.display = "flex";
	buildResultsList(tab);
	document.getElementById("loading-modal-div").style.display = "none";
}

function buildResultsList(tab) {
	let resultsList = document.getElementById("results-list-div");
	let package;
	fetch(apiUrl + `/${tab}`).then((response) => {
		if (!response.ok) {
			throw new Error('Network response was not ok');
		}
		return response.json();
	}).then((data) => {
		package = data;
	}).catch((error) => {
		console.error("Error:", error);
	});
	let i = 0;
	while (i < package.items.length)
	{
		let newResult = document.createElement("div");
		newResult.setAttribute("id", `result-${i}-div`);
		newResult.setAttribute("class", "div div--result");
		newResult.setAttribute("data-result", `${i}`);
		// titulo/id ou wtv TODO
		let resultData = document.createElement("h1");
		resultData.innerHTML = "Nome/ID/Wtv";
		newResult.appendChild(resultData);
		// outros dados TODO
		resultData = document.createElement("p");
		resultData.innerHTML = "wtv other info";
		newResult.appendChild(resultData);
		// imagem se for aplicável TODO
		resultData = document.createElement("img");
		resultData.setAttribute("img", "src da imagem");
		newResult.appendChild(resultData);
		// methods to be called on this item - if 1, button, else, dropdown
		let resultActions = document.createElement("div");
		resultActions.setAttribute("id", `result-${i}-actions-div`);
		resultActions.setAttribute("class", "div div--result-actions");
		if (package.methods.length == 1)
		{
			let button = document.createElement("button");
			button.setAttribute("value", package.methods[0]);
			resultActions.appendChild(button);
		}
		else
		{
			let select = document.createElement("select");
			select.setAttribute("id", `result-${i}-actions-select`);
			let n = 0;
			while (n < package.methods.length)
			{
				let option = document.createElement("option");
				option.setAttribute("value", package.methods[n].key);
				n++;
				select.appendChild(option);
			}
			resultActions.appendChild(select);
		}
		newResult.appendChild(resultActions);
		resultsList.appendChild(newResult);
		i++;
	}
	
}

function reservas() {
	let title = document.getElementById("tab-title");
	title.innerHTML = "Reservas";
	buildPagination();
}

function obras() {
	let title = document.getElementById("tab-title");
	title.innerHTML = "Obras";
	buildPagination();
}

function nucleos() {
	let title = document.getElementById("tab-title");
	title.innerHTML = "Núcleos";
	buildPagination();
}

function gestaoDeUsers() {
	let title = document.getElementById("tab-title");
	title.innerHTML = "Gestão de Utilizadores";
	buildPagination();
}

function buildPagination() {
	return ;
	const page = document.getElementById("tab-div");
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