// Bloom & Co. Global Cart Management
const BloomCart = {
    storageKey: 'bloom_cart_items',

    getItems() {
        const items = localStorage.getItem(this.storageKey);
        return items ? JSON.parse(items) : [];
    },

    saveItems(items) {
        localStorage.setItem(this.storageKey, JSON.stringify(items));
        window.dispatchEvent(new CustomEvent('cartUpdated', { detail: { items } }));
    },

    addItem(product) {
        const items = this.getItems();
        const existingItemIndex = items.findIndex(item => item.id == product.id);

        // Price is always passed as InvariantCulture number (dot as decimal separator)
        let parsedPrice = parseFloat(String(product.price).replace(/,/g, '.')) || 0;

        if (existingItemIndex > -1) {
            items[existingItemIndex].qty = (items[existingItemIndex].qty || 1) + 1;
        } else {
            items.push({
                id: product.id,
                name: product.name,
                price: parsedPrice || 0,
                img: product.img,
                qty: 1
            });
        }

        this.saveItems(items);
        this.showFeedback(product.name);
    },

    removeItem(productId) {
        let items = this.getItems();
        items = items.filter(item => item.id != productId);
        this.saveItems(items);
    },

    updateQuantity(productId, qty) {
        const items = this.getItems();
        const item = items.find(item => item.id == productId);
        if (item) {
            const count = parseInt(qty);
            if (count < 1) {
                this.removeItem(productId);
                return;
            }
            item.qty = count;
            this.saveItems(items);
        }
    },

    getTotal() {
        return this.getItems().reduce((total, item) => total + (item.price * item.qty), 0);
    },

    getCount() {
        return this.getItems().reduce((count, item) => count + item.qty, 0);
    },

    formatPrice(price) {
        return new Intl.NumberFormat("tr-TR", {
            style: "currency",
            currency: "TRY"
        }).format(Number(price) || 0);
    },

    showFeedback(productName) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: 'Sepete Eklendi! 🌸',
                text: `${productName} başarıyla sepetinize eklendi.`,
                icon: 'success',
                timer: 2000,
                showConfirmButton: false,
                toast: true,
                position: 'top-end',
                background: '#FFF8F5',
                color: '#3D2C33',
                iconColor: '#D4728A'
            });
        } else {
            console.log(`${productName} added to cart.`);
        }
    }
};

// Global UI Updates
$(document).ready(() => {
    const updateCartCounter = () => {
        const count = BloomCart.getCount();
        const counterEl = $('.cart-count-badge');
        if (count > 0) {
            if (counterEl.length === 0) {
                $('.header-icon[title="Sepetim"]').append(`<span class="cart-count-badge">${count}</span>`);
            } else {
                counterEl.text(count);
            }
        } else {
            counterEl.remove();
        }
    };

    window.addEventListener('cartUpdated', updateCartCounter);
    updateCartCounter();
});
