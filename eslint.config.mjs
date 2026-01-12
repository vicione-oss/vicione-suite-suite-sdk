import xoTypeScript from 'eslint-config-xo-typescript';

// Adjust configuration object according to our requirements
// https://github.com/xojs/eslint-config-xo-typescript/issues/88
const tsConfigurationObject = xoTypeScript[1];

// https://github.com/typescript-eslint/typescript-eslint/issues/9739#issuecomment-2296442418
tsConfigurationObject.languageOptions.parserOptions.projectService = {
    allowDefaultProject: ['eslint.config.mjs'],

    // Use of import.meta.dirname instead of ./ to fix config read error when opening a .ts file in VS although the change
    // still does not fix broken linting in VS, see https://developercommunity.microsoft.com/t/Use-updated-flat-config-eslintconfigj/10703354
    defaultProject: `${import.meta.dirname}/tsconfig.json`
};

const tsxConfigurationObject = xoTypeScript[xoTypeScript.length - 1];

// Include .ts files in liniting, required because file extension .tsx is configured by default
tsxConfigurationObject.files.push('**/*.ts');

export default [
    {
        ignores: [
            '**/wwwroot/js/*.js',
            '**/bin',
            'tests/**/*.js'
        ]
    },
    ...xoTypeScript,
    {
        rules: {
            'max-params': 'off',
            'no-unused-vars': 'error',
            curly: ['error', 'multi-or-nest', 'consistent'],
            '@stylistic/padded-blocks': 'off',
            '@stylistic/indent': ['error', 4],
            '@stylistic/indent-binary-ops': ['error', 4],
            '@stylistic/comma-dangle': ['error', 'never'],
            '@stylistic/function-paren-newline': ['error', 'consistent'],
            '@stylistic/object-curly-spacing': ['error', 'always'],
            '@typescript-eslint/no-empty-object-type': ['error', { allowInterfaces: 'with-single-extends' }],

            // Rules added to fix errors of kind "TypeError: Error while loading rule '...': Cannot read properties of undefined"
            '@typescript-eslint/dot-notation': ['error', { allowKeywords: true }],
            'no-empty-function': 'off',
            '@typescript-eslint/no-empty-function': ['error', { allow: [] }],
            '@typescript-eslint/no-unused-expressions': ['error', { allowShortCircuit: false }]
        }
    },
    {
        files: ['eslint.config.mjs'],
        rules: {
            '@typescript-eslint/no-unsafe-assignment': 'off',
            '@typescript-eslint/no-unsafe-call': 'off',
            '@typescript-eslint/naming-convention': 'off'
        }
    }
];
