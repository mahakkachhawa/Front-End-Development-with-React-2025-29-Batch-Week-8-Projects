import React from "react";
import { render, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import App from "../App";
import GiftMessage from "../components/GiftMessage";

const text = (container, selector) => container.querySelector(selector).textContent;

const clickTimes = (container, selector, times) => {
  for (let i = 0; i < times; i++) {
    fireEvent.click(container.querySelector(selector));
  }
};

const applyCoupon = (container, code) => {
  fireEvent.change(container.querySelector("#couponInput"), { target: { value: code } });
  fireEvent.click(container.querySelector("#applyCouponBtn"));
};

const typeGift = (container, value) => {
  fireEvent.change(container.querySelector("#giftInput"), { target: { value } });
};

// "Hidden" means not in the DOM (or hidden with the hidden attribute / display: none)
const isHidden = (element) =>
  element === null || element.hidden === true || element.style.display === "none";

describe("Product Order App", () => {
  test("verify_heading_and_both_components_rendering", () => {
    const { container } = render(<App />);
    expect(container.querySelector("h1#title")).toHaveTextContent("Product Order");
    expect(container.querySelector("p#functionalLabel")).toHaveTextContent(
      "Functional Component (useState)"
    );
    expect(container.querySelector("p#classLabel")).toHaveTextContent(
      "Class Component (this.state)"
    );
  });

  test("verify_product_details_and_initial_values", () => {
    const { container } = render(<App />);
    expect(container.querySelector("h2#productName")).toHaveTextContent("Wireless Headphones");
    expect(text(container, "#unitPrice")).toBe("500");
    expect(text(container, "#quantity")).toBe("1");
    expect(text(container, "#totalPrice")).toBe("500");
    expect(container.querySelector("button#increaseBtn").textContent.trim()).toBe("+");
    expect(container.querySelector("button#decreaseBtn").textContent.trim()).toBe("-");
    expect(container.querySelector("input#couponInput").value).toBe("");
    expect(container.querySelector("button#applyCouponBtn").textContent.trim()).toBe("Apply");
    expect(container.querySelector("#couponMessage")).not.toBeInTheDocument();
  });

  test("verify_quantity_stays_between_one_and_five_and_total_updates", () => {
    const { container } = render(<App />);
    clickTimes(container, "#decreaseBtn", 2);
    expect(text(container, "#quantity")).toBe("1");

    clickTimes(container, "#increaseBtn", 2);
    expect(text(container, "#quantity")).toBe("3");
    expect(text(container, "#totalPrice")).toBe("1500");

    clickTimes(container, "#increaseBtn", 6);
    expect(text(container, "#quantity")).toBe("5");
    expect(text(container, "#totalPrice")).toBe("2500");

    fireEvent.click(container.querySelector("#decreaseBtn"));
    expect(text(container, "#quantity")).toBe("4");
    expect(text(container, "#totalPrice")).toBe("2000");
  });

  test("verify_details_toggle_button_and_section", () => {
    const { container } = render(<App />);
    const btn = container.querySelector("button#detailsBtn");
    expect(btn.textContent.trim()).toBe("Show Details");
    expect(isHidden(container.querySelector("#productDetails"))).toBe(true);

    fireEvent.click(btn);
    expect(container.querySelector("#detailsBtn").textContent.trim()).toBe("Hide Details");
    expect(container.querySelector("div#productDetails")).toHaveTextContent(
      "Free delivery in 3-5 days"
    );

    fireEvent.click(container.querySelector("#detailsBtn"));
    expect(container.querySelector("#detailsBtn").textContent.trim()).toBe("Show Details");
    expect(isHidden(container.querySelector("#productDetails"))).toBe(true);
  });

  test("verify_valid_coupon_applies_ten_percent_discount", () => {
    const { container } = render(<App />);
    applyCoupon(container, "SAVE10");
    expect(container.querySelector("p#couponMessage").textContent).toBe(
      "Coupon applied: 10% off"
    );
    expect(text(container, "#totalPrice")).toBe("450");

    // Case-insensitive and spaces trimmed
    applyCoupon(container, "  save10 ");
    expect(text(container, "#couponMessage")).toBe("Coupon applied: 10% off");
    expect(text(container, "#totalPrice")).toBe("450");
  });

  test("verify_invalid_coupon_shows_message_and_no_discount", () => {
    const { container } = render(<App />);
    applyCoupon(container, "HELLO");
    expect(text(container, "#couponMessage")).toBe("Invalid coupon code");
    expect(text(container, "#totalPrice")).toBe("500");

    // An empty code is also invalid
    applyCoupon(container, "");
    expect(text(container, "#couponMessage")).toBe("Invalid coupon code");
    expect(text(container, "#totalPrice")).toBe("500");

    // A valid coupon followed by an invalid one removes the discount
    applyCoupon(container, "SAVE10");
    expect(text(container, "#totalPrice")).toBe("450");
    applyCoupon(container, "WRONG");
    expect(text(container, "#couponMessage")).toBe("Invalid coupon code");
    expect(text(container, "#totalPrice")).toBe("500");
  });

  test("verify_discounted_total_updates_when_quantity_changes", () => {
    const { container } = render(<App />);
    applyCoupon(container, "SAVE10");
    fireEvent.click(container.querySelector("#increaseBtn"));
    expect(text(container, "#quantity")).toBe("2");
    expect(text(container, "#totalPrice")).toBe("900");

    fireEvent.click(container.querySelector("#increaseBtn"));
    expect(text(container, "#totalPrice")).toBe("1350");

    fireEvent.click(container.querySelector("#decreaseBtn"));
    fireEvent.click(container.querySelector("#decreaseBtn"));
    expect(text(container, "#quantity")).toBe("1");
    expect(text(container, "#totalPrice")).toBe("450");
  });

  test("verify_gift_message_is_a_class_component_with_initial_state", () => {
    // Class components extend React.Component, so they carry isReactComponent
    expect(GiftMessage.prototype && GiftMessage.prototype.isReactComponent).toBeTruthy();

    const { container } = render(<App />);
    const input = container.querySelector("input#giftInput");
    expect(input).toBeInTheDocument();
    expect(input.value).toBe("");
    expect(container.querySelector("p#giftCounter").textContent).toBe("0 / 50");
    expect(container.querySelector("button#saveGiftBtn").textContent.trim()).toBe(
      "Save Gift Message"
    );
    expect(container.querySelector("#giftError")).not.toBeInTheDocument();
    expect(container.querySelector("#savedGift")).not.toBeInTheDocument();
  });

  test("verify_gift_counter_updates_and_shows_actual_length_over_fifty", () => {
    const { container } = render(<App />);
    typeGift(container, "Happy Birthday!");
    expect(container.querySelector("#giftInput").value).toBe("Happy Birthday!");
    expect(text(container, "#giftCounter")).toBe("15 / 50");

    typeGift(container, "a".repeat(50));
    expect(text(container, "#giftCounter")).toBe("50 / 50");

    typeGift(container, "a".repeat(52));
    expect(text(container, "#giftCounter")).toBe("52 / 50");
  });

  test("verify_saving_gift_message_and_error_handling", () => {
    const { container } = render(<App />);

    // Empty message -> error
    fireEvent.click(container.querySelector("#saveGiftBtn"));
    expect(container.querySelector("p#giftError").textContent).toBe("Please write a message");
    expect(container.querySelector("#savedGift")).not.toBeInTheDocument();

    // Whitespace-only message -> error
    typeGift(container, "     ");
    fireEvent.click(container.querySelector("#saveGiftBtn"));
    expect(text(container, "#giftError")).toBe("Please write a message");
    expect(container.querySelector("#savedGift")).not.toBeInTheDocument();

    // Valid message -> saved, error disappears
    typeGift(container, "Happy Birthday!");
    fireEvent.click(container.querySelector("#saveGiftBtn"));
    expect(container.querySelector("p#savedGift").textContent).toBe(
      "Gift message: Happy Birthday!"
    );
    expect(container.querySelector("#giftError")).not.toBeInTheDocument();
  });
});