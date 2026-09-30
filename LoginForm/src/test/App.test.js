import React from "react";
import { render, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import App from "../App";

const typeEmail = (container, value) => {
  fireEvent.change(container.querySelector("#emailInput"), { target: { value } });
};

const typePassword = (container, value) => {
  fireEvent.change(container.querySelector("#passwordInput"), { target: { value } });
};

const clickLogin = (container) => {
  fireEvent.click(container.querySelector("#loginBtn"));
};

describe("Login Form UI App", () => {
  test("verify_heading_rendering", () => {
    const { container } = render(<App />);
    const title = container.querySelector("h1#title");
    expect(title).toBeInTheDocument();
    expect(title).toHaveTextContent("Login");
  });

  test("verify_initial_form_rendering", () => {
    const { container } = render(<App />);
    const email = container.querySelector("input#emailInput");
    const password = container.querySelector("input#passwordInput");
    expect(email).toBeInTheDocument();
    expect(email).toHaveAttribute("type", "email");
    expect(email.value).toBe("");
    expect(password).toBeInTheDocument();
    expect(password).toHaveAttribute("type", "password");
    expect(password.value).toBe("");
    expect(container.querySelector("button#loginBtn").textContent.trim()).toBe("Login");

    // Nothing to show yet, and no <form> element is used
    expect(container.querySelector("#passwordStrength")).not.toBeInTheDocument();
    expect(container.querySelector("#emailError")).not.toBeInTheDocument();
    expect(container.querySelector("#passwordError")).not.toBeInTheDocument();
    expect(container.querySelector("#successMsg")).not.toBeInTheDocument();
    expect(container.querySelector("form")).not.toBeInTheDocument();
  });

  test("verify_show_hide_password_toggle", () => {
    const { container } = render(<App />);
    const toggle = container.querySelector("button#togglePassword");
    expect(toggle.textContent.trim()).toBe("Show");
    expect(container.querySelector("#passwordInput")).toHaveAttribute("type", "password");

    fireEvent.click(toggle);
    expect(container.querySelector("#togglePassword").textContent.trim()).toBe("Hide");
    expect(container.querySelector("#passwordInput")).toHaveAttribute("type", "text");

    fireEvent.click(container.querySelector("#togglePassword"));
    expect(container.querySelector("#togglePassword").textContent.trim()).toBe("Show");
    expect(container.querySelector("#passwordInput")).toHaveAttribute("type", "password");
  });

  test("verify_strength_hidden_when_password_is_empty", () => {
    const { container } = render(<App />);
    expect(container.querySelector("#passwordStrength")).not.toBeInTheDocument();

    typePassword(container, "abc");
    expect(container.querySelector("p#passwordStrength")).toBeInTheDocument();

    typePassword(container, "");
    expect(container.querySelector("#passwordStrength")).not.toBeInTheDocument();
  });

  test("verify_password_strength_levels", () => {
    const { container } = render(<App />);
    const cases = [
      ["12345", "Weak"],
      ["123456", "Medium"],
      ["123456789", "Medium"],
      ["1234567890", "Strong"],
      ["12345678901234", "Strong"],
    ];
    cases.forEach(([pwd, label]) => {
      typePassword(container, pwd);
      expect(container.querySelector("#passwordStrength").textContent).toBe(label);
    });
  });

  test("verify_invalid_email_shows_only_email_error", () => {
    const { container } = render(<App />);
    typeEmail(container, "userexample.com");
    typePassword(container, "secret123");
    clickLogin(container);

    expect(container.querySelector("p#emailError").textContent).toBe("Enter a valid email");
    expect(container.querySelector("#passwordError")).not.toBeInTheDocument();
    expect(container.querySelector("#successMsg")).not.toBeInTheDocument();
  });

  test("verify_short_password_shows_only_password_error", () => {
    const { container } = render(<App />);
    typeEmail(container, "user@example.com");
    typePassword(container, "12345");
    clickLogin(container);

    expect(container.querySelector("p#passwordError").textContent).toBe(
      "Password must be at least 6 characters"
    );
    expect(container.querySelector("#emailError")).not.toBeInTheDocument();
    expect(container.querySelector("#successMsg")).not.toBeInTheDocument();
  });

  test("verify_both_errors_can_appear_together", () => {
    const { container } = render(<App />);
    // Both fields left empty
    clickLogin(container);
    expect(container.querySelector("#emailError").textContent).toBe("Enter a valid email");
    expect(container.querySelector("#passwordError").textContent).toBe(
      "Password must be at least 6 characters"
    );
    expect(container.querySelector("#successMsg")).not.toBeInTheDocument();
  });

  test("verify_successful_login_message_and_no_errors", () => {
    const { container } = render(<App />);
    typeEmail(container, "user@example.com");
    typePassword(container, "secret123");
    clickLogin(container);

    expect(container.querySelector("p#successMsg").textContent).toBe(
      "Login successful for user@example.com"
    );
    expect(container.querySelector("#emailError")).not.toBeInTheDocument();
    expect(container.querySelector("#passwordError")).not.toBeInTheDocument();
  });

  test("verify_typing_clears_that_fields_error_and_the_success_message", () => {
    const { container } = render(<App />);

    // Both errors showing
    clickLogin(container);
    expect(container.querySelector("#emailError")).toBeInTheDocument();
    expect(container.querySelector("#passwordError")).toBeInTheDocument();

    // Typing in the email field clears only the email error
    typeEmail(container, "user@example.com");
    expect(container.querySelector("#emailError")).not.toBeInTheDocument();
    expect(container.querySelector("#passwordError")).toBeInTheDocument();

    // Typing in the password field clears the password error
    typePassword(container, "secret123");
    expect(container.querySelector("#passwordError")).not.toBeInTheDocument();

    // Success message disappears as soon as either field changes
    clickLogin(container);
    expect(container.querySelector("#successMsg")).toBeInTheDocument();
    typePassword(container, "secret1234");
    expect(container.querySelector("#successMsg")).not.toBeInTheDocument();

    clickLogin(container);
    expect(container.querySelector("#successMsg")).toBeInTheDocument();
    typeEmail(container, "other@example.com");
    expect(container.querySelector("#successMsg")).not.toBeInTheDocument();
  });
});