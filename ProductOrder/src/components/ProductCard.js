import React, { useState } from "react";

const UNIT_PRICE = 500;
const MIN_QUANTITY = 1;
const MAX_QUANTITY = 5;
const COUPON_CODE = "SAVE10";
const DISCOUNT_PERCENT = 10;

// FUNCTIONAL COMPONENT: state is managed with the useState hook
function ProductCard() {
  const [quantity, setQuantity] = useState(MIN_QUANTITY);
  const [couponInput, setCouponInput] = useState("");
  const [couponApplied, setCouponApplied] = useState(false);
  const [couponMessage, setCouponMessage] = useState("");
  const [showDetails, setShowDetails] = useState(false);

  const handleIncrease = () => {
    setQuantity((prev) => Math.min(prev + 1, MAX_QUANTITY));
  };

  const handleDecrease = () => {
    setQuantity((prev) => Math.max(prev - 1, MIN_QUANTITY));
  };

  const handleCouponChange = (event) => {
    setCouponInput(event.target.value);
  };

  const handleApplyCoupon = () => {
    const code = couponInput.trim().toUpperCase(); // case-insensitive, spaces trimmed
    if (code === COUPON_CODE) {
      setCouponApplied(true);
      setCouponMessage(`Coupon applied: ${DISCOUNT_PERCENT}% off`);
    } else {
      setCouponApplied(false); // an invalid code means no discount
      setCouponMessage("Invalid coupon code");
    }
  };

  const handleToggleDetails = () => {
    setShowDetails((prev) => !prev);
  };

  // Derived values (calculated from state on every render)
  const subtotal = UNIT_PRICE * quantity;
  const discount = couponApplied ? (subtotal * DISCOUNT_PERCENT) / 100 : 0;
  const totalPrice = subtotal - discount;

  return (
    <section className="section">
      <p id="functionalLabel" className="badge badge-functional">
        Functional Component (useState)
      </p>

      <h2 id="productName" className="product-name">Wireless Headphones</h2>
      <p className="price-line">
        Unit price: ₹ <span id="unitPrice">{UNIT_PRICE}</span>
      </p>

      <div className="stepper">
        <button id="decreaseBtn" className="step-btn" onClick={handleDecrease}>-</button>
        <span id="quantity" className="quantity">{quantity}</span>
        <button id="increaseBtn" className="step-btn" onClick={handleIncrease}>+</button>
      </div>

      <div className="coupon-row">
        <input
          id="couponInput"
          className="text-input"
          type="text"
          placeholder="Coupon code"
          value={couponInput}
          onChange={handleCouponChange}
        />
        <button id="applyCouponBtn" className="small-btn" onClick={handleApplyCoupon}>
          Apply
        </button>
      </div>
      {couponMessage && (
        <p
          id="couponMessage"
          className={couponApplied ? "message message-ok" : "message message-error"}
        >
          {couponMessage}
        </p>
      )}

      <p className="total-line">
        Total: ₹ <span id="totalPrice">{totalPrice}</span>
      </p>

      <button id="detailsBtn" className="link-btn" onClick={handleToggleDetails}>
        {showDetails ? "Hide Details" : "Show Details"}
      </button>
      {showDetails && (
        <div id="productDetails" className="details">Free delivery in 3-5 days</div>
      )}
    </section>
  );
}

export default ProductCard;