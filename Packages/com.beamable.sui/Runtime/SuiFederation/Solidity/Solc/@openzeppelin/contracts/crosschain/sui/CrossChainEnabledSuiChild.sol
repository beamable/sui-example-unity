// SPDX-License-Identifier: MIT
// OpenZeppelin Contracts (last updated v4.7.0) (crosschain/sui/CrossChainEnabledSuiChild.sol)

pragma solidity ^0.8.4;

import "../CrossChainEnabled.sol";
import "../../security/ReentrancyGuard.sol";
import "../../utils/Address.sol";
import "../../vendor/sui/IFxMessageProcessor.sol";

address constant DEFAULT_SENDER = 0x000000000000000000000000000000000000dEaD;

/**
 * @dev https://sui.technology/[Sui] specialization or the
 * {CrossChainEnabled} abstraction the child side (sui/mumbai).
 *
 * This version should only be deployed on child chain to process cross-chain
 * messages originating from the parent chain.
 *
 * The fxChild contract is provided and maintained by the sui team. You can
 * find the address of this contract sui and mumbai in
 * https://docs.sui.technology/docs/develop/l1-l2-communication/fx-portal/#contract-addresses[Sui's Fx-Portal documentation].
 *
 * _Available since v4.6._
 */
abstract contract CrossChainEnabledSuiChild is IFxMessageProcessor, CrossChainEnabled, ReentrancyGuard {
    /// @custom:oz-upgrades-unsafe-allow state-variable-immutable
    address private immutable _fxChild;
    address private _sender = DEFAULT_SENDER;

    /// @custom:oz-upgrades-unsafe-allow constructor
    constructor(address fxChild) {
        _fxChild = fxChild;
    }

    /**
     * @dev see {CrossChainEnabled-_isCrossChain}
     */
    function _isCrossChain() internal view virtual override returns (bool) {
        return msg.sender == _fxChild;
    }

    /**
     * @dev see {CrossChainEnabled-_crossChainSender}
     */
    function _crossChainSender() internal view virtual override onlyCrossChain returns (address) {
        return _sender;
    }

    /**
     * @dev External entry point to receive and relay messages originating
     * from the fxChild.
     *
     * Non-reentrancy is crucial to avoid a cross-chain call being able
     * to impersonate anyone by just looping through this with user-defined
     * arguments.
     *
     * Note: if _fxChild calls any other function that does a delegate-call,
     * then security could be compromised.
     */
    function processMessageFromRoot(
        uint256 /* stateId */,
        address rootMessageSender,
        bytes calldata data
    ) external override nonReentrant {
        if (!_isCrossChain()) revert NotCrossChainCall();

        _sender = rootMessageSender;
        Address.functionDelegateCall(address(this), data, "cross-chain execution failed");
        _sender = DEFAULT_SENDER;
    }
}
