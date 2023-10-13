//PHOTOS SETUP
const ourPhotosSectionDOM = document.getElementById("our-photos");
const searchPhotosSectionDOM = document.getElementById("search-photos");
const searchPhotoInputDOM = document.getElementById("photoSearchInput");
if (searchPhotoInputDOM) {
	searchPhotoInputDOM.addEventListener("keyup", loadSearchPhotosSection);
}
loadOurPhotosSection();

// MESSAGES SETUP
const sendMsgBtnDOM = document.getElementById("send-msg-btn");
const contactUsSectionDOM = document.getElementById("contact-us");
if (sendMsgBtnDOM) {
	sendMsgBtnDOM.addEventListener("click", (event) => {
		event.preventDefault();
		sendMessage();
	})
}


// FUNCTIONS
function loadOurPhotosSection() {
	if (ourPhotosSectionDOM) {
		const photosLoaderDOM = ourPhotosSectionDOM.querySelector(".loader");
		const photosListDOM = ourPhotosSectionDOM.querySelector(".data-list");
		const photosEmptyDOM = ourPhotosSectionDOM.querySelector(".empty-data-list");


		axios.get('/api/Photos/GetAll')
			.then(response => {
				photosLoaderDOM.classList.add("d-none");
				if (response.data.length == 0) {
					photosEmptyDOM.classList.remove("d-none");
				} else {
					photosListDOM.innerHTML = "";
					for (i = 0; i < response.data.length && i < 4; i++) {
						let photo = response.data[i];
						photosListDOM.innerHTML += generatePhotoCardDOM(photo);
					}
					photosListDOM.classList.remove("d-none");
					photosLoaderDOM.classList.add("d-none");
				}
			});
	}
}
function loadSearchPhotosSection() {
	
	if (searchPhotosSectionDOM) {
		const searchLoaderDOM = searchPhotosSectionDOM.querySelector(".loader");
		const searchListDOM = searchPhotosSectionDOM.querySelector(".data-list");
		const searchEmptyDOM = searchPhotosSectionDOM.querySelector(".empty-data-list");

		let name = searchPhotoInputDOM.value;
		let emptyInputUrl = '/api/Photos/GetAll';

		searchLoaderDOM.classList.remove("d-none");
		axios.get(name ? '/api/Photos/GetAllContaining' : emptyInputUrl,
			{
				params: { name }
			}).then(response => {
				searchListDOM.innerHTML = "";
				searchLoaderDOM.classList.add("d-none");

				if (response.data.length == 0) {
					searchEmptyDOM.classList.remove("d-none");
					searchListDOM.classList.add("d-none");
				} else {
					searchEmptyDOM.classList.add("d-none");
					response.data.forEach(photo => {
						searchListDOM.innerHTML += generatePhotoCardDOM(photo);
					});
					searchListDOM.classList.remove("d-none");
				}
			});
	}
}
function generatePhotoCardDOM(photo) {
	let categories = [];
	photo.categories.forEach( (category) => categories.push(category.name));
	return `
    <div class="col">
		<div class="card h-100 shadow">
			<img src="uploads/${photo.imgPath}" class="card-img-top photo-img" alt="">
			<div class="card-body">
				<h5 class="card-title">${photo.title}</h5>
				<p class="card-text">${photo.description}</p>
			</div>
			<div class="card-footer">
				<small class="text-body-secondary"> Categories: ${categories.Length > 0 ? categories.join(", ") : "no categories found"}</small>
			</div>
		</div>
	</div>
    `
}

function sendMessage() {
	// SETUP
	const emailAlertDOM = contactUsSectionDOM.querySelector("#mail-alert");
	const bodyAlertDOM = contactUsSectionDOM.querySelector("#message-alert");
	const emailDOM = document.getElementById("email");
	const bodyDOM = document.getElementById("message");

	// LOGIC
	sendMsgBtnDOM.classList.add("disabled");
	sendMsgBtnDOM.innerHTML = "Sending..."
	let email = emailDOM.value.trim();
	let body = bodyDOM.value.trim();

	if (fieldsCheckPassed()) {
		let message = { email, body };
		axios.post("/api/Messages/Send", message)
			.then(() => {
				emailDOM.value = "";
				bodyDOM.value = "";
				
			}).catch(err => {
				printServerErrors(err.response.data.errors);
			});
	}
	sendMsgBtnDOM.innerHTML = "Send message";
	sendMsgBtnDOM.classList.remove("disabled");

	// BASIC FIELDS CHECK
	function fieldsCheckPassed() {
		let result = true;
		if (email == "") {
			result = false;
			emailAlertDOM.classList.remove("d-none");
		} else {
			emailAlertDOM.classList.add("d-none");
		}
		if (body == "") {
			result = false;
			bodyAlertDOM.classList.remove("d-none");
		} else {
			bodyAlertDOM.classList.add("d-none");
		}
		return result;
	}
	function printServerErrors(errors) {
		if (errors.Body) {
			bodyAlertDOM.classList.remove("d-none")
			const bodyErrors = bodyAlertDOM.querySelector(".errors");
			bodyErrors.innerHTML = "";
			errors.Body.forEach(msg => {
				bodyErrors.innerHTML += ` ${msg} `;
			});
		}
		if (errors.Email) {
			emailAlertDOM.classList.remove("d-none");
			const emailErrors = emailAlertDOM.querySelector(".errors");
			emailErrors.innerHTML = "";
			errors.Email.forEach(msg => {
				emailErrors.innerHTML += ` ${msg} `;
			});
		}
	}
}

