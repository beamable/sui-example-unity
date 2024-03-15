module beamable_sui_example::{{toLowerCase module_name}} {

use sui::url::{Self, Url};
use std::string::{Self, String, utf8};
use sui::object::{Self, UID};
use sui::tx_context::{Self, TxContext, sender};
use sui::transfer;
use sui::package;
use sui::display;
use std::vector as vec;

/// Ensure NFT metadata (Attributes) vector properties length
const EVecLengthMismatch: u64 = 1;

/// Type that marks Capability to create new item
struct GameAdminCap has key { id: UID }

/// NFT one-time witness (guaranteed to have at most one instance), name matches the module name
struct {{toUpperCase module_name}} has drop {}

/// NFT metadata
struct Attribute has store, copy, drop {
    name: String,
    value: String
}

/// NFT Struct
struct {{toStructName module_name}} has key, store {
    id: UID,
    name: String,
    description: String,
    url: Url,
    attributes: vector<Attribute>
}

/// Called on contract publish, defines NFT display properties
fun init(otw: {{toUpperCase module_name}}, ctx: &mut TxContext) {
    let keys = vector[
        utf8(b"name"),
        utf8(b"description"),
        utf8(b"url")
        ];
    let values = vector[
        utf8(b"{name}"),
        utf8(b"{description}"),
        utf8(b"{url}")
        ];

    let publisher = package::claim(otw, ctx);
    let display = display::new_with_fields<{{toStructName module_name}}>(&publisher, keys, values, ctx);
    display::update_version(&mut display);
    transfer::public_transfer(publisher, sender(ctx));
    transfer::public_transfer(display, sender(ctx));
    transfer::transfer(GameAdminCap {id: object::new(ctx)}, tx_context::sender(ctx));
}

/// Constructs NFT object
fun new(
    name: vector<u8>,
    description: vector<u8>,
    url: vector<u8>,
    names: vector<String>,
    values: vector<String>,
    ctx: &mut TxContext): {{toStructName module_name}} {
        let len = vec::length(&names);
        assert!(len == vec::length(&values), EVecLengthMismatch);

        let item = {{toStructName module_name}} {
            id: object::new(ctx),
            name: string::utf8(name),
            description: string::utf8(description),
            url: url::new_unsafe_from_bytes(url),
            attributes: vec::empty()
        };

    let i = 0;
    while (i < len) {
        vec::push_back(&mut item.attributes, Attribute {
        name: *vec::borrow(&names, i),
        value: *vec::borrow(&values, i)
        });
        i = i + 1;
    };
    item
}

/// NFT mint function
public entry fun create(
    _: &GameAdminCap,
    player: address,
    name: vector<u8>,
    description: vector<u8>,
    url: vector<u8>,
    {{#each customFields}}
    {{this}}: vector<u8>,
    {{/each}}
    names: vector<String>,
    values: vector<String>,
    ctx: &mut TxContext) {
    let item = new(name,description,url,{{#each fields}}{{this}},{{/each}}names,values,ctx);
    transfer::transfer(item, player);
    }
}