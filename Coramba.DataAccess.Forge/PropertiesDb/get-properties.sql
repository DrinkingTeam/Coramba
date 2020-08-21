select
    e.*,
    name.value [name],
    parent.value [parent],
    instanceof_objid.value instanceof_objid,
    hastable.value hastable,
    viewable_in.value viewable_in,
    xref.value xref,
    is_doc_property.value is_doc_property,
    schema_name.value schema_name,
    schema_version.value schema_version,
    hyperlink.value hyperlink,
    node_flags.value node_flags
from
    _objects_id e
    left join (
        select
            eav.entity_id,
            cast(v.value as varchar) value
        from
            _objects_eav eav
            join _objects_val v on v.id = eav.value_id
        where
            eav.attribute_id = 1
    ) name on name.entity_id = e.id
    left join (
        select
            eav.entity_id,
            cast(v.value as varchar) value
        from
            _objects_eav eav
            join _objects_val v on v.id = eav.value_id
        where
            eav.attribute_id = 3
    ) parent on parent.entity_id = e.id
    left join (
        select
            eav.entity_id,
            cast(v.value as varchar) value
        from
            _objects_eav eav
            join _objects_val v on v.id = eav.value_id
        where
            eav.attribute_id = 4
    ) instanceof_objid on instanceof_objid.entity_id = e.id
    left join (
        select
            eav.entity_id,
            cast(v.value as varchar) value
        from
            _objects_eav eav
            join _objects_val v on v.id = eav.value_id
        where
            eav.attribute_id = 1
    ) hastable on hastable.entity_id = e.id
    left join (
        select
            eav.entity_id,
            cast(v.value as varchar) value
        from
            _objects_eav eav
            join _objects_val v on v.id = eav.value_id
        where
            eav.attribute_id = 6
    ) viewable_in on viewable_in.entity_id = e.id
    left join (
        select
            eav.entity_id,
            cast(v.value as varchar) value
        from
            _objects_eav eav
            join _objects_val v on v.id = eav.value_id
        where
            eav.attribute_id = 7
    ) xref on xref.entity_id = e.id
    left join (
        select
            eav.entity_id,
            cast(v.value as varchar) value
        from
            _objects_eav eav
            join _objects_val v on v.id = eav.value_id
        where
            eav.attribute_id = 8
    ) is_doc_property on is_doc_property.entity_id = e.id
    left join (
        select
            eav.entity_id,
            cast(v.value as varchar) value
        from
            _objects_eav eav
            join _objects_val v on v.id = eav.value_id
        where
            eav.attribute_id = 9
    ) schema_name on schema_name.entity_id = e.id
    left join (
        select
            eav.entity_id,
            cast(v.value as varchar) value
        from
            _objects_eav eav
            join _objects_val v on v.id = eav.value_id
        where
            eav.attribute_id = 10
    ) schema_version on schema_version.entity_id = e.id
    left join (
        select
            eav.entity_id,
            cast(v.value as varchar) value
        from
            _objects_eav eav
            join _objects_val v on v.id = eav.value_id
        where
            eav.attribute_id = 11
    ) hyperlink on hyperlink.entity_id = e.id
    left join (
        select
            eav.entity_id,
            cast(v.value as varchar) value
        from
            _objects_eav eav
            join _objects_val v on v.id = eav.value_id
        where
            eav.attribute_id = 12
    ) node_flags on node_flags.entity_id = e.id
--where
--    viewable_in.value is null
limit 1000