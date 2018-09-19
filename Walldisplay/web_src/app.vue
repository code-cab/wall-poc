<template>
    <div>
        <nav>
            <ul>
                <li><router-link class="btn btn-default" to="/">User statistics</router-link></li>
                <li><router-link class="btn btn-default" to="/statistics">Statistics</router-link></li>
            </ul>
        </nav>
        <div>
            <router-view></router-view>
        </div>
    </div>
</template>