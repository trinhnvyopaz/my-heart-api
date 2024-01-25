<template>

    <b-col cols="12" :class="{'col-lg-3 col-md-6' : showTiles}" class="pr-1 pl-1 mb-2" style="max-height: 200px">
        <router-link class="tile-hover" tag="a" :to="{ path: '/toolbox-edit', query: { id: id }}">
            <b-card class="bg-default card-toolbox" no-body>
                <div class="card-body d-flex">
                    <div class="align-self-center text-center mx-auto">
                        <b-img style="max-width: 70px" class="d-inline-block" :src="image" v-if="image" />
                        <div class="mt-2">
                            <p style="font-size: 1em;" class="card-title">{{ name }}</p>                            
                        </div>

                    </div>
                </div>
                <p style="font-size: .8em; position:relative; bottom:1rem;" class="card-text text-muted mx-auto">{{description}}</p>
                <p style="font-size: .8em; position:relative; bottom:1rem;" class="card-text text-muted mx-auto">{{ type }}</p>
            </b-card>
        </router-link>
    </b-col>

</template>

<script>
export default {
        props: ['count', 'image', 'name', 'type', 'id', 'showTiles','description']
}
</script>

<style>

    .card.card-toolbox
    {
        padding: 0;
        height: 100%;
    }

    .card.card-toolbox img
    {
        max-width: 50%;
    }

    .card.card-toolbox strong.card-title
    {
        white-space: nowrap;
        color: #000;
    }

</style>
