const SensorToken = artifacts.require("SensorToken");

module.exports = function (deployer) {
    deployer.deploy(SensorToken);
};
