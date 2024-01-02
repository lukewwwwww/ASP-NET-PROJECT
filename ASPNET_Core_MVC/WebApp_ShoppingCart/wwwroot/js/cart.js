// Initialize Total Price
UpdatePriceDisplay();

// Handle checkout button
let checkoutLink = document.getElementById("checkoutLink");

function disableCheckout(cartQuantity) {
	if (cartQuantity == 0) {
		console.log("Cart is empty.")
		checkoutLink.classList.add("disable-link");
	}
}

// Add 'Input' EventListeners to all QuantityChange fields
let quantityBtns = document.getElementsByClassName("btn-changeQuantity");

for (let i = 0; i < quantityBtns.length; i++) {
	quantityBtns[i].addEventListener("input", function (e) {
		let xhr = new XMLHttpRequest();
		xhr.open("POST", "/Cart/UpdateCartItem");
		xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

		console.log("productId=" + e.currentTarget.dataset.productId + "&quantity=" + e.currentTarget.value)
		xhr.send("productId=" + e.currentTarget.dataset.productId + "&quantity=" + e.currentTarget.value);
		UpdatePriceDisplay();

		xhr.onreadystatechange = function () {
			if (xhr.readyState === XMLHttpRequest.DONE) {
				if (xhr.status === 200) {
					// Succeed
					console.log("Change cart successfully.");
				} else {
					// Report Error
					console.error("Error changing cart. Status code: " + xhr.status);
				}
			}
		}
	})
}

// Add 'Click' EventListeners to all Remove buttons
let removeBtns = document.getElementsByClassName("btn-removeItem");

for (let i = 0; i < removeBtns.length; i++) {
	removeBtns[i].addEventListener("click", function (e) {
		let xhr = new XMLHttpRequest();
		xhr.open("POST", "/Cart/RemoveCartItem");
		xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

		console.log("productId=" + e.currentTarget.dataset.productId)
		xhr.send("productId=" + e.currentTarget.dataset.productId);
		RemoveDivById(e.currentTarget.dataset.productId);
		UpdatePriceDisplay();

		xhr.onreadystatechange = function () {
			if (xhr.readyState === XMLHttpRequest.DONE) {
				if (xhr.status === 200) {
					// Succeed
					console.log("Removed item successfully.");
					console.log("cart count is" + JSON.parse(this.responseText).cartCount)
					disableCheckout(JSON.parse(this.responseText).cartCount);
				} else {
					// Report Error
					console.error("Error removing item. Status code: " + xhr.status);
				}
			}
		};
	})
}

// Helper functions
function RemoveDivById(productId) {
	let divId = "item-container-" + productId;
	let div = document.getElementById(divId);
	div.remove();
}

function UpdatePriceDisplay() {
	let cartItems = document.getElementsByClassName("btn-changeQuantity");
	let sum = 0;

	for (let i = 0; i < cartItems.length; i++) {
		sum += cartItems[i].dataset.productPrice * cartItems[i].value;
	}

	let priceDisplay = document.getElementById("total-price-display");
	priceDisplay.innerHTML = sum.toFixed(2);
}