//PHOTOS SETUP
const ourPhotosDOM = document.getElementById("our-photos");
const searchPhotosDOM = document.getElementById("search-photos");
const searchPhotoInputDOM = document.getElementById("photoSearchInput");
if (searchPhotoInputDOM) {
	searchPhotoInputDOM.addEventListener("keyup", searchPhotos);
}
loadPhotos();
// MESSAGES SETUP
const sendMsgBtn = document.getElementById("send-msg-btn");
const contactUsDOM = document.getElementById("contact-us");
if (sendMsgBtn) {
	sendMsgBtn.addEventListener("click", (event) => {
		event.preventDefault();
		sendMessage();
	})
}


// FUNCTIONS
function loadPhotos() {
	if (ourPhotosDOM) {
		const photosLoaderDOM = document.querySelector("#our-photos-data .loader");
		const photosListDOM = document.querySelector("#our-photos-data .data-list");
		const photosEmptyDOM = document.querySelector("#our-photos-data .empty-data-list");


		axios.get('/api/Photos/GetAll')
			.then(response => {
				photosLoaderDOM.classList.add("d-none");
				if (response.data.length == 0) {
					photosEmptyDOM.classList.remove("d-none");
				} else {
					photosListDOM.innerHTML = "";
					for (i = 0; i < response.data.length && i < 4; i++) {
						let photo = response.data[i];
						photosListDOM.innerHTML += generatePhotoDOM(photo);
					}
					photosListDOM.classList.remove("d-none");
					photosLoaderDOM.classList.add("d-none");
				}
			});
	}
}
function searchPhotos() {
	
	if (searchPhotosDOM) {
		const searchLoaderDOM = searchPhotosDOM.querySelector(".loader");
		const searchListDOM = searchPhotosDOM.querySelector(".data-list");
		const searchEmptyDOM = searchPhotosDOM.querySelector(".empty-data-list");

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
						searchListDOM.innerHTML += generatePhotoDOM(photo);
					});
					searchListDOM.classList.remove("d-none");
				}
			});
	}
}
function generatePhotoDOM(photo) {
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
				<small class="text-body-secondary"> Categories: ${categories.join(", ")}</small>
			</div>
		</div>
	</div>
    `
}

function sendMessage() {
	sendMsgBtn.classList.add("disabled");
	sendMsgBtn.innerHTML = "Sending..."
	const emailDOM = document.getElementById("email");
	const bodyDOM = document.getElementById("message");
	let email = emailDOM.value.trim();
	let body = bodyDOM.value.trim();

	if (isMessageValid()) {
		let message = { email, body };
		axios.post("/api/Messages/Send", message)
			.then(() => {
				emailDOM.value = "";
				bodyDOM.value = "";
				alert("Message sent!");
			}).catch(err => {
				console.log(err);
			});
	}
	sendMsgBtn.innerHTML = "Send message";
	sendMsgBtn.classList.remove("disabled");

	function isMessageValid() {
		let result = true;
		const emailAlertDOM = contactUsDOM.querySelector("#mail-alert");
		const bodyAlertDOM = contactUsDOM.querySelector("#message-alert");
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
}

