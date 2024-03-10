module beamable_sui_example::{{toLowerCase module_name}} {
use std::option;
use sui::coin;
use sui::transfer;
use sui::tx_context::{Self, TxContext};

struct {{toUpperCase module_name}} has drop {}

fun init(otw: {{toUpperCase module_name}}, ctx: &mut TxContext) {
    let decimal = 0;
    let symbol = b"{{symbol}}";
    let name = b"{{name}}";
    let desc = b"{{description}}";
    let (treasury, metadata) = coin::create_currency(otw, decimal, symbol, name, desc, option::none(), ctx);
    transfer::public_freeze_object(metadata);
    transfer::public_transfer(treasury, tx_context::sender(ctx))
}

public entry fun mint(
    treasury: &mut coin::TreasuryCap<{{toUpperCase module_name}}>,
    amount: u64,
    recipient: address,
    ctx: &mut TxContext
    ) {
    coin::mint_and_transfer(treasury, amount, recipient, ctx)
}


public entry fun burn(treasury: &mut coin::TreasuryCap<{{toUpperCase module_name}}>, coin: coin::Coin<{{toUpperCase module_name}}>) {
    coin::burn(treasury, coin);
    }
}