import React from "react";
import { render, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import App from "../App";
import Counter from "../components/Counter";
import Greeting from "../components/Greeting";

const text = (container, selector) => container.querySelector(selector).textContent;

const clickTimes = (container, selector, times) => {
  for (let i = 0; i < times; i++) {
    fireEvent.click(container.querySelector(selector));
  }
};

const typeName = (container, value) => {
  fireEvent.change(container.querySelector("#nameInput"), { target: { value } });
};

const greet = (container, value) => {
  typeName(container, value);
  fireEvent.click(container.querySelector("#greetBtn"));
};

describe("Counter and Greeting App", () => {
  test("verify_heading_and_both_component_labels", () => {
    const { container } = render(<App />);
    expect(container.querySelector("h1#title")).toHaveTextContent("Counter and Greeting");
    expect(container.querySelector("p#functionalLabel")).toHaveTextContent(
      "Functional Component (useState)"
    );
    expect(container.querySelector("p#classLabel")).toHaveTextContent(
      "Class Component (this.state)"
    );
  });

  test("verify_initial_values_of_both_components", () => {
    const { container } = render(<App />);
    expect(text(container, "#count")).toBe("0");
    expect(container.querySelector("button#increaseBtn").textContent.trim()).toBe("+");
    expect(container.querySelector("button#decreaseBtn").textContent.trim()).toBe("-");
    expect(container.querySelector("button#resetBtn").textContent.trim()).toBe("Reset");
    expect(container.querySelector("#countMessage")).not.toBeInTheDocument();
    expect(container.querySelector("input#nameInput").value).toBe("");
    expect(container.querySelector("button#greetBtn").textContent.trim()).toBe("Greet");
    expect(container.querySelector("#greeting")).not.toBeInTheDocument();
    expect(container.querySelector("#nameError")).not.toBeInTheDocument();
  });

  test("verify_counter_is_functional_and_greeting_is_class_component", () => {
    // Class components carry isReactComponent on their prototype
    expect(Greeting.prototype && Greeting.prototype.isReactComponent).toBeTruthy();
    expect(Counter.prototype && Counter.prototype.isReactComponent).toBeFalsy();
  });

  test("verify_increase_button_increments_the_count", () => {
    const { container } = render(<App />);
    fireEvent.click(container.querySelector("#increaseBtn"));
    expect(text(container, "#count")).toBe("1");
    clickTimes(container, "#increaseBtn", 2);
    expect(text(container, "#count")).toBe("3");
  });

  test("verify_decrease_button_never_goes_below_zero", () => {
    const { container } = render(<App />);
    clickTimes(container, "#decreaseBtn", 3);
    expect(text(container, "#count")).toBe("0");

    clickTimes(container, "#increaseBtn", 2);
    fireEvent.click(container.querySelector("#decreaseBtn"));
    expect(text(container, "#count")).toBe("1");
    clickTimes(container, "#decreaseBtn", 4);
    expect(text(container, "#count")).toBe("0");
  });

  test("verify_high_count_message_shows_at_five_or_more", () => {
    const { container } = render(<App />);
    clickTimes(container, "#increaseBtn", 4);
    expect(text(container, "#count")).toBe("4");
    expect(container.querySelector("#countMessage")).not.toBeInTheDocument();

    fireEvent.click(container.querySelector("#increaseBtn"));
    expect(text(container, "#count")).toBe("5");
    expect(container.querySelector("p#countMessage").textContent).toBe("High count!");

    fireEvent.click(container.querySelector("#decreaseBtn"));
    expect(text(container, "#count")).toBe("4");
    expect(container.querySelector("#countMessage")).not.toBeInTheDocument();
  });

  test("verify_reset_button_sets_count_to_zero_and_hides_message", () => {
    const { container } = render(<App />);
    clickTimes(container, "#increaseBtn", 7);
    expect(text(container, "#count")).toBe("7");
    expect(container.querySelector("#countMessage")).toBeInTheDocument();

    fireEvent.click(container.querySelector("#resetBtn"));
    expect(text(container, "#count")).toBe("0");
    expect(container.querySelector("#countMessage")).not.toBeInTheDocument();
  });

  test("verify_name_input_updates_but_greeting_is_not_shown_until_click", () => {
    const { container } = render(<App />);
    typeName(container, "Asha");
    expect(container.querySelector("#nameInput").value).toBe("Asha");
    expect(container.querySelector("#greeting")).not.toBeInTheDocument();
    expect(container.querySelector("#nameError")).not.toBeInTheDocument();
  });

  test("verify_greet_with_name_shows_trimmed_greeting", () => {
    const { container } = render(<App />);
    greet(container, "  Asha  ");
    expect(container.querySelector("p#greeting").textContent).toBe("Hello, Asha!");
    expect(container.querySelector("#nameError")).not.toBeInTheDocument();
  });

  test("verify_empty_name_shows_error_and_error_clears_after_success", () => {
    const { container } = render(<App />);

    // Empty name -> error
    fireEvent.click(container.querySelector("#greetBtn"));
    expect(container.querySelector("p#nameError").textContent).toBe("Please enter your name");
    expect(container.querySelector("#greeting")).not.toBeInTheDocument();

    // Whitespace-only name -> error
    greet(container, "     ");
    expect(text(container, "#nameError")).toBe("Please enter your name");
    expect(container.querySelector("#greeting")).not.toBeInTheDocument();

    // Valid name -> greeting shown, error removed
    greet(container, "Ravi");
    expect(text(container, "#greeting")).toBe("Hello, Ravi!");
    expect(container.querySelector("#nameError")).not.toBeInTheDocument();
  });
});