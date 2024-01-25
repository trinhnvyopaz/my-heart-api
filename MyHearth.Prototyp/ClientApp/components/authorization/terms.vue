<template>
    <div class="layer">
        <div class="content-container" style="text-align:left; font-family: arial">

            <h1 v-localize="{i: 'terms.title'}" />
            <h3 v-localize="{i: 'terms.header'}" /><br />

            <p>
                Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas lorem. Mauris elementum mauris vitae tortor. In sem justo, commodo ut, suscipit at, pharetra vitae, orci. Donec iaculis gravida nulla. Nullam dapibus fermentum ipsum. Maecenas lorem. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Integer rutrum, orci vestibulum ullamcorper ultricies, lacus quam ultricies odio, vitae placerat pede sem sit amet enim. Nullam eget nisl. Fusce tellus odio, dapibus id fermentum quis, suscipit id erat. Nullam lectus justo, vulputate eget mollis sed, tempor sed magna. Fusce tellus odio, dapibus id fermentum quis, suscipit id erat. Donec quis nibh at felis congue commodo. Nulla pulvinar eleifend sem. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae.
            </p>

            <!--<h3 v-localize="{i: 'terms.title'}" />
            <p v-localize="{i: 'terms.header'}" />

            <h4 v-localize="{i: 'terms.paragraphOne.title'}" />
            <p v-localize="{i: 'terms.paragraphOne.lineOne'}" />
            <p v-localize="{i: 'terms.paragraphOne.lineTwo'}" />
            <p v-localize="{i: 'terms.paragraphOne.lineThree'}" />
            <p v-localize="{i: 'terms.paragraphOne.lineFour'}" />
            <p v-localize="{i: 'terms.paragraphOne.lineFive'}" />
            <p v-localize="{i: 'terms.paragraphOne.lineSix'}" />
            <p v-localize="{i: 'terms.paragraphOne.lineSeven'}" />

            <h4 v-localize="{i: 'terms.paragraphTwo.title'}" />
            <p v-localize="{i: 'terms.paragraphTwo.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphThree.title'}" />
            <p v-localize="{i: 'terms.paragraphThree.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphFour.title'}" />
            <p v-localize="{i: 'terms.paragraphFour.lineOne'}" />


            <h4 v-localize="{i: 'terms.paragraphFive.title'}" />
            <p v-localize="{i: 'terms.paragraphFive.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphFiveOne.title'}" />
            <p v-localize="{i: 'terms.paragraphFiveOne.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphFiveTwo.title'}" />
            <p v-localize="{i: 'terms.paragraphFiveTwo.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphFiveThree.title'}" />
            <p v-localize="{i: 'terms.paragraphFiveThree.lineOne'}" />
            <p v-localize="{i: 'terms.paragraphFiveThree.lineTwo'}" />
            <p v-localize="{i: 'terms.paragraphFiveThree.lineThree'}" />
            <p v-localize="{i: 'terms.paragraphFiveThree.lineFour'}" />

            <h4 v-localize="{i: 'terms.paragraphFiveFour.title'}" />
            <p v-localize="{i: 'terms.paragraphFiveFour.lineOne'}" />
            <p v-localize="{i: 'terms.paragraphFiveFour.lineTwo'}" />
            <p v-localize="{i: 'terms.paragraphFiveFour.lineThree'}" />
            <p v-localize="{i: 'terms.paragraphFiveFour.lineFour'}" />
            <p v-localize="{i: 'terms.paragraphFiveFour.lineFive'}" />
            <p v-localize="{i: 'terms.paragraphFiveFour.lineSix'}" />

            <h4 v-localize="{i: 'terms.paragraphFiveFive.title'}" />
            <p v-localize="{i: 'terms.paragraphFiveFive.lineOne'}" />
            <p v-localize="{i: 'terms.paragraphFiveFive.lineTwo'}" />
            <p v-localize="{i: 'terms.paragraphFiveFive.lineThree'}" />
            <p v-localize="{i: 'terms.paragraphFiveFive.lineFour'}" />

            <h4 v-localize="{i: 'terms.paragraphFiveSix.title'}" />
            <p v-localize="{i: 'terms.paragraphFiveSix.lineOne'}" />
            <p v-localize="{i: 'terms.paragraphFiveSix.lineTwo'}" />
            <p v-localize="{i: 'terms.paragraphFiveSix.lineThree'}" />

            <h4 v-localize="{i: 'terms.paragraphFiveSeven.title'}" />
            <p v-localize="{i: 'terms.paragraphFiveSeven.lineOne'}" />
            <p v-localize="{i: 'terms.paragraphFiveSeven.lineTwo'}" />

            <h4 v-localize="{i: 'terms.paragraphFiveEight.title'}" />
            <p v-localize="{i: 'terms.paragraphFiveEight.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphFiveNine.title'}" />
            <p v-localize="{i: 'terms.paragraphFiveNine.lineOne'}" />
            <p v-localize="{i: 'terms.paragraphFiveNine.lineTwo'}" />
            <p v-localize="{i: 'terms.paragraphFiveNine.lineThree'}" />

            <h4 v-localize="{i: 'terms.paragraphFiveTen.title'}" />
            <p v-localize="{i: 'terms.paragraphFiveTen.lineOne'}" />
            <p v-localize="{i: 'terms.paragraphFiveTen.lineTwo'}" />
            <p v-localize="{i: 'terms.paragraphFiveTen.lineThree'}" />

            <h4 v-localize="{i: 'terms.paragraphSix.title'}" />
            <p v-localize="{i: 'terms.paragraphSix.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphSeven.title'}" />
            <p v-localize="{i: 'terms.paragraphSeven.lineOne'}" />
            <p v-localize="{i: 'terms.paragraphSeven.lineTwo'}" />

            <h4 v-localize="{i: 'terms.paragraphEight.title'}" />
            <p v-localize="{i: 'terms.paragraphEight.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphNine.title'}" />
            <p v-localize="{i: 'terms.paragraphNine.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphTen.title'}" />
            <p v-localize="{i: 'terms.paragraphTen.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphTenOne.title'}" />
            <p v-localize="{i: 'terms.paragraphTenOne.lineOne'}" />
            <p v-localize="{i: 'terms.paragraphTenOne.lineTwo'}" />

            <h4 v-localize="{i: 'terms.paragraphEleven.title'}" />
            <p v-localize="{i: 'terms.paragraphEleven.lineOne'}" />
            <p v-localize="{i: 'terms.paragraphEleven.lineTwo'}" />

            <h4 v-localize="{i: 'terms.paragraphTwelve.title'}" />
            <p v-localize="{i: 'terms.paragraphTwelve.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphThrirteen.title'}" />
            <p v-localize="{i: 'terms.paragraphThrirteen.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphFourteen.title'}" />
            <p v-localize="{i: 'terms.paragraphFourteen.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphFifteen.title'}" />
            <p v-localize="{i: 'terms.paragraphFifteen.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphSixteen.title'}" />
            <p v-localize="{i: 'terms.paragraphSixteen.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphSeventeen.title'}" />
            <p v-localize="{i: 'terms.paragraphSeventeen.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphEighteen.title'}" />
            <p v-localize="{i: 'terms.paragraphEighteen.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphNineteen.title'}" />
            <p v-localize="{i: 'terms.paragraphNineteen.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphTwenty.title'}" />
            <p v-localize="{i: 'terms.paragraphTwenty.lineOne'}" />

            <h4 v-localize="{i: 'terms.paragraphTwentyOne.title'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineOne'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineTwo'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineThree'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineFour'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineFive'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineSix'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineSeven'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineEight'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineNine'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineTen'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineEleven'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineTwelve'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineThirteen'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineFourteen'}" />
            <p v-localize="{i: 'terms.paragraphTwentyOne.lineFifteen'}" />-->
        </div>

    </div>
</template>


<script>

    export default {
        data() {
            return {

            }
        },

        methods: {

        },

    }
</script>


<style scoped>
</style>
