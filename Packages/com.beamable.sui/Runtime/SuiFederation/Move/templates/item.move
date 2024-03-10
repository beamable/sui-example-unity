module beamable_sui_example::{{toLowerCase module_name}} {

use sui::url::{Self, Url};
use std::string::{Self, String, utf8};
use sui::object::{Self, UID};
use sui::tx_context::{Self, TxContext, sender};
use sui::transfer;
use sui::package;
use sui::display;
use std::vector as vec;

const EVecLengthMismatch: u64 = 1;

struct GameAdminCap has key { id: UID }

struct {{toUpperCase module_name}} has drop {}

struct Attribute has store, copy, drop {
    name: String,
    value: String
}

struct {{toStructName module_name}} has key, store {
    id: UID,
    name: String,
    description: String,
    url: Url,
    {{#each customFields}}
    {{this}}: String,
    {{/each}}
    attributes: vector<Attribute>
}

fun init(otw: {{toUpperCase module_name}}, ctx: &mut TxContext) {
    let keys = vector[
        utf8(b"name"),
        utf8(b"description"),
        utf8(b"url"),
        {{#each customFields}}
        utf8(b"{{this}}"),
        {{/each}}
        ];
    let values = vector[
        utf8(b"{name}"),
        utf8(b"{description}"),
        utf8(b"{url}"),
        {{#each customFields}}
        utf8(b"{{this}}"),
        {{/each}}
        ];

    let publisher = package::claim(otw, ctx);
    let display = display::new_with_fields<{{toStructName module_name}}>(&publisher, keys, values, ctx);
    display::update_version(&mut display);
    transfer::public_transfer(publisher, sender(ctx));
    transfer::public_transfer(display, sender(ctx));
    transfer::transfer(GameAdminCap {id: object::new(ctx)}, tx_context::sender(ctx));
}

    public fun update_name(
        nft: &mut {{toStructName module_name}},
        new_name: vector<u8>,
        _: &mut TxContext
        ) {
        nft.name = string::utf8(new_name)
    }

    public fun update_description(
        nft: &mut {{toStructName module_name}},
        new_description: vector<u8>,
        _: &mut TxContext
        ) {
        nft.description = string::utf8(new_description)
    }

    public fun update_url(
        nft: &mut {{toStructName module_name}},
        new_url: vector<u8>,
        _: &mut TxContext
        ) {
        nft.url = url::new_unsafe_from_bytes(new_url)
    }

{{#each customFields}}
    public fun update_{{this}}(
        nft: &mut {{toStructName module_name}},
        new_{{this}}: vector<u8>,
        _: &mut TxContext
        ) {
        nft.{{this}} = string::utf8(new_{{this}})
    }

{{/each}}


fun new(
    name: vector<u8>,
    description: vector<u8>,
    url: vector<u8>,
    {{#each customFields}}
    {{this}}: vector<u8>,
    {{/each}}
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
            {{#each customFields}}
            {{this}}: string::utf8({{this}}),
            {{/each}}
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