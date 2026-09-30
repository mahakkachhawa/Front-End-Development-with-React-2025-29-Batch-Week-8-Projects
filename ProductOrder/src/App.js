import React from "react";
import ProductCard from "./components/ProductCard";
import GiftMessage from "./components/GiftMessage";

function App() {
  return (
    <div className="page">
      <div className="order-card">
        <h1 id="title" className="order-title">Product Order</h1>
        <ProductCard />
        <GiftMessage />
      </div>
    </div>
  );
}

export default App;