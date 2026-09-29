import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import App from "../App";

describe("Ticket Counter App", () => {
  test("verify_heading_rendering", () => {
    const { container } = render(<App />);
    const title = container.querySelector("h1#title");
    expect(title).toBeInTheDocument();
    expect(title).toHaveTextContent("Ticket Counter");
  });

  test("verify_initial_ticket_count_is_zero", () => {
    const { container } = render(<App />);
    const count = container.querySelector("span#ticketCount");
    expect(count).toBeInTheDocument();
    expect(count.textContent).toBe("0");
  });

  test("verify_ticket_label_rendering", () => {
    const { container } = render(<App />);
    const label = container.querySelector("p#ticketLabel");
    expect(label).toBeInTheDocument();
    expect(label).toHaveTextContent("Tickets booked:");
  });

  test("verify_label_appears_before_count", () => {
    const { container } = render(<App />);
    const label = container.querySelector("#ticketLabel");
    const count = container.querySelector("#ticketCount");
    // DOCUMENT_POSITION_FOLLOWING means `count` comes after `label` in the DOM
    expect(label.compareDocumentPosition(count) & Node.DOCUMENT_POSITION_FOLLOWING).toBeTruthy();
  });

  test("verify_add_ticket_button_rendering", () => {
    const { container } = render(<App />);
    const addBtn = container.querySelector("button#addTicket");
    expect(addBtn).toBeInTheDocument();
    expect(addBtn).toHaveTextContent("Add Ticket");
  });

  test("verify_reset_button_rendering", () => {
    const { container } = render(<App />);
    const resetBtn = container.querySelector("button#resetTickets");
    expect(resetBtn).toBeInTheDocument();
    expect(resetBtn).toHaveTextContent("Reset");
  });

  test("verify_single_click_increases_count_by_one", () => {
    const { container } = render(<App />);
    fireEvent.click(container.querySelector("#addTicket"));
    expect(container.querySelector("#ticketCount").textContent).toBe("1");
  });

  test("verify_three_clicks_show_count_three", () => {
    const { container } = render(<App />);
    const addBtn = container.querySelector("#addTicket");
    fireEvent.click(addBtn);
    fireEvent.click(addBtn);
    fireEvent.click(addBtn);
    expect(container.querySelector("#ticketCount").textContent).toBe("3");
  });

  test("verify_reset_sets_count_back_to_zero", () => {
    const { container } = render(<App />);
    const addBtn = container.querySelector("#addTicket");
    fireEvent.click(addBtn);
    fireEvent.click(addBtn);
    fireEvent.click(addBtn);
    expect(container.querySelector("#ticketCount").textContent).toBe("3");

    fireEvent.click(container.querySelector("#resetTickets"));
    expect(container.querySelector("#ticketCount").textContent).toBe("0");
  });

  test("verify_counting_continues_from_zero_after_reset", () => {
    const { container } = render(<App />);
    const addBtn = container.querySelector("#addTicket");
    const resetBtn = container.querySelector("#resetTickets");

    fireEvent.click(addBtn);
    fireEvent.click(addBtn);
    fireEvent.click(resetBtn);
    fireEvent.click(resetBtn); // resetting at 0 must keep it at 0
    expect(container.querySelector("#ticketCount").textContent).toBe("0");

    fireEvent.click(addBtn);
    expect(container.querySelector("#ticketCount").textContent).toBe("1");
  });
});