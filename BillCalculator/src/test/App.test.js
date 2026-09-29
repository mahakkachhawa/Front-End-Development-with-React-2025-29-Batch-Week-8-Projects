import React from "react";
import { render, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import App from "../App";

const enterBill = (container, value) => {
  fireEvent.change(container.querySelector("#billInput"), { target: { value } });
};

const text = (container, selector) => container.querySelector(selector).textContent;

describe("Bill Calculator App", () => {
  test("verify_heading_rendering", () => {
    const { container } = render(<App />);
    const title = container.querySelector("h1#title");
    expect(title).toBeInTheDocument();
    expect(title).toHaveTextContent("Bill Calculator");
  });

  test("verify_bill_input_is_number_type_and_empty", () => {
    const { container } = render(<App />);
    const input = container.querySelector("input#billInput");
    expect(input).toBeInTheDocument();
    expect(input).toHaveAttribute("type", "number");
    expect(input.value).toBe("");
  });

  test("verify_initial_values_on_screen", () => {
    const { container } = render(<App />);
    expect(text(container, "#tipPercent")).toBe("0");
    expect(text(container, "#peopleCount")).toBe("1");
    expect(text(container, "#tipAmount")).toBe("0.00");
    expect(text(container, "#totalAmount")).toBe("0.00");
    expect(text(container, "#perPerson")).toBe("0.00");
  });

  test("verify_tip_buttons_rendering", () => {
    const { container } = render(<App />);
    expect(container.querySelector("button#tip10Btn").textContent.trim()).toBe("10%");
    expect(container.querySelector("button#tip15Btn").textContent.trim()).toBe("15%");
    expect(container.querySelector("button#tip20Btn").textContent.trim()).toBe("20%");
  });

  test("verify_tip_buttons_update_selected_percentage", () => {
    const { container } = render(<App />);
    fireEvent.click(container.querySelector("#tip15Btn"));
    expect(text(container, "#tipPercent")).toBe("15");
    fireEvent.click(container.querySelector("#tip20Btn"));
    expect(text(container, "#tipPercent")).toBe("20");
    fireEvent.click(container.querySelector("#tip10Btn"));
    expect(text(container, "#tipPercent")).toBe("10");
  });

  test("verify_people_buttons_and_minimum_of_one", () => {
    const { container } = render(<App />);
    expect(container.querySelector("#increasePeople").textContent.trim()).toBe("+");
    expect(container.querySelector("#decreasePeople").textContent.trim()).toBe("-");

    fireEvent.click(container.querySelector("#increasePeople"));
    fireEvent.click(container.querySelector("#increasePeople"));
    expect(text(container, "#peopleCount")).toBe("3");

    fireEvent.click(container.querySelector("#decreasePeople"));
    expect(text(container, "#peopleCount")).toBe("2");

    // Try to go below 1
    fireEvent.click(container.querySelector("#decreasePeople"));
    fireEvent.click(container.querySelector("#decreasePeople"));
    fireEvent.click(container.querySelector("#decreasePeople"));
    expect(text(container, "#peopleCount")).toBe("1");
  });

  test("verify_calculation_for_example_case", () => {
    // bill 1000, tip 10%, people 2 -> tip 100.00, total 1100.00, per person 550.00
    const { container } = render(<App />);
    enterBill(container, "1000");
    fireEvent.click(container.querySelector("#tip10Btn"));
    fireEvent.click(container.querySelector("#increasePeople"));
    expect(text(container, "#tipAmount")).toBe("100.00");
    expect(text(container, "#totalAmount")).toBe("1100.00");
    expect(text(container, "#perPerson")).toBe("550.00");
  });

  test("verify_results_are_rounded_to_two_decimal_places", () => {
    // bill 100, tip 15%, people 3 -> tip 15.00, total 115.00, per person 38.33
    const { container } = render(<App />);
    enterBill(container, "100");
    fireEvent.click(container.querySelector("#tip15Btn"));
    fireEvent.click(container.querySelector("#increasePeople"));
    fireEvent.click(container.querySelector("#increasePeople"));
    expect(text(container, "#tipAmount")).toBe("15.00");
    expect(text(container, "#totalAmount")).toBe("115.00");
    expect(text(container, "#perPerson")).toBe("38.33");
  });

  test("verify_empty_bill_shows_zero_and_no_nan", () => {
    const { container } = render(<App />);
    fireEvent.click(container.querySelector("#tip20Btn"));
    fireEvent.click(container.querySelector("#increasePeople"));

    // Bill was never entered
    expect(text(container, "#tipAmount")).toBe("0.00");
    expect(text(container, "#totalAmount")).toBe("0.00");
    expect(text(container, "#perPerson")).toBe("0.00");

    // Type a bill, then clear it again
    enterBill(container, "500");
    expect(text(container, "#totalAmount")).toBe("600.00");
    enterBill(container, "");
    expect(text(container, "#tipAmount")).toBe("0.00");
    expect(text(container, "#totalAmount")).toBe("0.00");
    expect(text(container, "#perPerson")).toBe("0.00");
    expect(container.textContent).not.toContain("NaN");
  });

  test("verify_reset_button_restores_initial_values", () => {
    const { container } = render(<App />);
    const resetBtn = container.querySelector("button#resetBtn");
    expect(resetBtn).toBeInTheDocument();
    expect(resetBtn.textContent.trim()).toBe("Reset");

    enterBill(container, "800");
    fireEvent.click(container.querySelector("#tip20Btn"));
    fireEvent.click(container.querySelector("#increasePeople"));
    fireEvent.click(container.querySelector("#increasePeople"));
    fireEvent.click(resetBtn);

    expect(container.querySelector("#billInput").value).toBe("");
    expect(text(container, "#tipPercent")).toBe("0");
    expect(text(container, "#peopleCount")).toBe("1");
    expect(text(container, "#tipAmount")).toBe("0.00");
    expect(text(container, "#totalAmount")).toBe("0.00");
    expect(text(container, "#perPerson")).toBe("0.00");
  });
});