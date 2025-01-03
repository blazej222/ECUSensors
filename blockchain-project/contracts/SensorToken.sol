// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

interface IERC20 {

	 /**
     * @dev Returns the balance of tokens held by the given address (`who`).
     * @param who The address to query the balance for.
     * @return The number of tokens owned by the given address.
     */
    function balanceOf(address who) external view returns (uint256);

    /**
     * @dev Transfers a specified amount of tokens (`value`) to the address `to`.
     * Must return `true` if the transaction succeeds.
     * @param to The address of the token recipient.
     * @param value The number of tokens to transfer.
     * @return Returns `true` if the operation is successful.
     */
    function transfer(address to, uint256 value) external returns (bool);

    /**
     * @dev Checks how many tokens the owner (`owner`) has approved for use by the address (`spender`).
     * @param tokenOwner The address of the token owner.
     * @param spender The address allowed to spend the tokens.
     * @return The number of tokens the `spender` is allowed to use.
     */
    function allowance(address tokenOwner, address spender) external view returns (uint256);

    /**
     * @dev Approves the address (`spender`) to spend a specified number of tokens (`value`)
     * on behalf of the contract owner.
     * Must return `true` if the operation is successful.
     * @param spender The address allowed to spend the tokens.
     * @param value The number of tokens to approve.
     * @return Returns `true` if the operation is successful.
     */
    function approve(address spender, uint256 value) external returns (bool);

    /**
     * @dev Transfers tokens from one address (`from`) to another (`to`) if previously approved.
     * Must return `true` if the operation is successful.
     * @param from The address from which the tokens are sent.
     * @param to The address to which the tokens are sent.
     * @param value The number of tokens to transfer.
     * @return Returns `true` if the operation is successful.
     */
    function transferFrom(address from, address to, uint256 value) external returns (bool);

    /**
     * @dev Returns the total number of tokens available in the system.
     * @return The total number of tokens in circulation.
     */
    function totalSupply() external view returns (uint256);
}

contract SensorToken is IERC20 {
    string public name = "Sensor Token";
    string public symbol = "SEN";
    uint8 public decimals = 18; 
    uint256 public totalSupply = 1000000 * 10 ** uint256(decimals);
    address public owner;
    mapping(address => uint256) public balances;
    mapping(address => mapping(address => uint256)) public allowed;

    constructor() {
        owner = msg.sender;
        balances[owner] = totalSupply;
    }
	
	function rewardSensor(address sensor, uint256 amount) external onlyOwner {
        require(sensor != address(0), "Invalid sensor address");
        require(balances[owner] >= amount, "Insufficient tokens in the contract");
        balances[owner] -= amount;
        balances[sensor] += amount;
        emit Transfer(owner, sensor, amount);
    }

    function transfer(address to, uint256 value) public override returns (bool) {
        require(balances[msg.sender] >= value, "Insufficient balance");
        balances[msg.sender] -= value;
        balances[to] += value;
        emit Transfer(msg.sender, to, value);
        return true;
    }

    function balanceOf(address who) public view override returns (uint256) {
        return balances[who];
    }

    function allowance(address tokenOwner, address spender) public view override returns (uint256) {
        return allowed[tokenOwner][spender];
    }

    function approve(address spender, uint256 value) public override returns (bool) {
        allowed[msg.sender][spender] = value;
        emit Approval(msg.sender, spender, value);
        return true;
    }

    function transferFrom(address from, address to, uint256 value) public override returns (bool) {
        require(balances[from] >= value, "Insufficient balance");
        require(allowed[from][msg.sender] >= value, "Not allowed to spend");
        balances[from] -= value;
        balances[to] += value;
        allowed[from][msg.sender] -= value;
        emit Transfer(from, to, value);
        return true;
    }

    modifier onlyOwner() {
        require(msg.sender == owner);
        _;
    }
    event Transfer(address indexed from, address indexed to, uint256 value);
    event Approval(address indexed owner, address indexed spender, uint256 value);
}
