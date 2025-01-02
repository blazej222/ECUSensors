// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";

contract SensorToken is ERC20 {
    address public admin;

    constructor() ERC20("SensorToken", "SNT") {
        admin = msg.sender;
        _mint(msg.sender, 1000000 * 10 ** decimals()); // 1M tokenów dla admina
    }

    function rewardSensor(address sensor, uint256 amount) external {
        require(msg.sender == admin, "Only admin can reward sensors");
        _transfer(admin, sensor, amount);
    }
}
