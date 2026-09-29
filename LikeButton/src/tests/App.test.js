import React from "react";
import { render, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import App from "../App";

describe("Class Like Button App", () => {
  test("verify_heading_rendering", () => {
    const { container } = render(<App />);
    const title = container.querySelector("h1#title");
    expect(title).toBeInTheDocument();
    expect(title).toHaveTextContent("Class Like Button");
  });

  test("verify_component_type_label_rendering", () => {
    const { container } = render(<App />);
    const type = container.querySelector("p#componentType");
    expect(type).toBeInTheDocument();
    expect(type).toHaveTextContent("Class Component");
  });

  test("verify_app_is_a_class_component", () => {
    // Class components extend React.Component, so they carry isReactComponent
    expect(App.prototype && App.prototype.isReactComponent).toBeTruthy();
  });

  test("verify_initial_like_count_is_zero", () => {
    const { container } = render(<App />);
    const count = container.querySelector("span#likeCount");
    expect(count).toBeInTheDocument();
    expect(count.textContent).toBe("0");
  });

  test("verify_initial_button_text_is_like", () => {
    const { container } = render(<App />);
    const btn = container.querySelector("button#likeBtn");
    expect(btn).toBeInTheDocument();
    expect(btn.textContent.trim()).toBe("Like");
  });

  test("verify_initial_status_message", () => {
    const { container } = render(<App />);
    const status = container.querySelector("p#likeStatus");
    expect(status).toBeInTheDocument();
    expect(status).toHaveTextContent("You have not liked this post");
  });

  test("verify_first_click_increases_count_and_shows_unlike", () => {
    const { container } = render(<App />);
    fireEvent.click(container.querySelector("#likeBtn"));
    expect(container.querySelector("#likeCount").textContent).toBe("1");
    expect(container.querySelector("#likeBtn").textContent.trim()).toBe("Unlike");
  });

  test("verify_first_click_updates_status_message", () => {
    const { container } = render(<App />);
    fireEvent.click(container.querySelector("#likeBtn"));
    expect(container.querySelector("#likeStatus")).toHaveTextContent("You liked this post");
  });

  test("verify_second_click_restores_initial_state", () => {
    const { container } = render(<App />);
    const btn = container.querySelector("#likeBtn");
    fireEvent.click(btn); // like
    fireEvent.click(btn); // unlike
    expect(container.querySelector("#likeCount").textContent).toBe("0");
    expect(container.querySelector("#likeBtn").textContent.trim()).toBe("Like");
    expect(container.querySelector("#likeStatus")).toHaveTextContent(
      "You have not liked this post"
    );
  });

  test("verify_repeated_toggling_keeps_state_consistent", () => {
    const { container } = render(<App />);
    const btn = container.querySelector("#likeBtn");
    fireEvent.click(btn);
    fireEvent.click(btn);
    fireEvent.click(btn); // three clicks -> liked
    expect(container.querySelector("#likeCount").textContent).toBe("1");
    expect(container.querySelector("#likeBtn").textContent.trim()).toBe("Unlike");
    expect(container.querySelector("#likeStatus")).toHaveTextContent("You liked this post");

    fireEvent.click(btn); // four clicks -> not liked
    expect(container.querySelector("#likeCount").textContent).toBe("0");
    expect(container.querySelector("#likeBtn").textContent.trim()).toBe("Like");
  });
});