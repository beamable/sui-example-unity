(()=>{"use strict";var t={};(()=>{var i=t;Object.defineProperty(i,"__esModule",{value:!0}),i.SuiCapObjects=i.SuiCapObject=i.SuiKeys=i.SuiTransactionResult=i.SuiViewObject=i.SuiObject=i.SuiCoinBalance=i.SuiBalance=void 0,i.SuiBalance=class{constructor(){this.coins=[]}},i.SuiCoinBalance=class{constructor(t,i){this.coinType=t,this.total=i}},i.SuiObject=class{constructor(t,i,s,e,c){this.objectId=t,this.digest=i,this.version=s,this.content=e,this.display=c}toView(){var t,i,e,c,n;const o=new s(this.objectId);if("moveObject"===(null===(t=this.content)||void 0===t?void 0:t.dataType)){const t=null===(i=this.content)||void 0===i?void 0:i.type.split("::");t.length>=3?o.type=t[1]:o.type=null===(e=this.content)||void 0===e?void 0:e.type;const s=null===(c=this.display)||void 0===c?void 0:c.data;null!=s&&(o.name=s.name,o.description=s.description,o.image_url=s.url);const a=null===(n=this.content)||void 0===n?void 0:n.fields;if(null!=a){const t=a.attributes;null!=t&&Array.isArray(t)&&t.forEach((t=>{const i=t;if(null!=i){const t=i.fields.name,s=i.fields.value;o.addAttribute(t,s)}}))}}return o}};class s{constructor(t){this.attributes=[],this.objectId=t}addAttribute(t,i){const s={Name:t,Value:i};this.attributes.push(s)}}i.SuiViewObject=s,i.SuiTransactionResult=class{},i.SuiKeys=class{},i.SuiCapObject=class{constructor(t,i){this.Id=t,this.Name=i}},i.SuiCapObjects=class{constructor(){this.GameAdminCaps=[],this.TreasuryCaps=[]}}})(),module.exports=t})();