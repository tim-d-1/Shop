// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract ShopCheckout {
    address public owner;

    event OrderPaid(uint256 indexed orderId, address indexed buyer, uint256 amount);

    constructor() {
        owner = msg.sender;
    }
    function payForOrder(uint256 orderId) external payable {
        require(msg.value > 0, "Must send ETH to pay");
        emit OrderPaid(orderId, msg.sender, msg.value);
    }

    function withdraw() external {
        require(msg.sender == owner, "Only admin can withdraw");

        uint balance = address(this).balance;
        (bool success, ) = payable(owner).call{value: balance}("");
        require(success, "Transfer failed");
    }
}